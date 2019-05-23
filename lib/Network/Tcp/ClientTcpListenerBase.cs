using System;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Muwesome.Common;
using Muwesome.Network.Tcp.Filters;
using Muwesome.Packet.IO;
using Pipelines.Sockets.Unofficial;

namespace Muwesome.Network.Tcp {
  /// <summary>Represents a client TCP listener for packet communications.</summary>
  public abstract class ClientTcpListenerBase<TClient> : IClientTcpListener<TClient>, IDisposable {
    private readonly int maxClientPacketSize;
    private TcpListener listener;
    private int isRunning;

    /// <summary>Initializes a new instance of the <see cref="ClientTcpListenerBase{T}"/> class.</summary>
    public ClientTcpListenerBase(IPEndPoint endPoint, int maxClientPacketSize) {
      this.maxClientPacketSize = maxClientPacketSize;
      this.SourceEndPoint = endPoint;
    }

    /// <summary>Factory for a pipeline encryptor.</summary>
    protected delegate IPipelineEncryptor PipelineEncryptorFactory(PipeWriter writer);

    /// <summary>Factory for a pipeline decryptor.</summary>
    protected delegate IPipelineDecryptor PipelineDecryptorFactory(PipeReader reader);

    /// <inheritdoc />
    public event EventHandler<LifecycleEventArgs> LifecycleStarted;

    /// <inheritdoc />
    public event EventHandler<LifecycleEventArgs> LifecycleEnded;

    /// <inheritdoc />
    public event EventHandler<ClientSocketAcceptEventArgs> ClientAccept;

    /// <inheritdoc />
    public event EventHandler<ClientConnectionEstablishedEventArgs> ClientConnectionEstablished;

    /// <inheritdoc />
    public event EventHandler<ClientConnectedEventArgs<TClient>> ClientConnected;

    /// <inheritdoc />
    public Task ShutdownTask { get; private set; } = Task.CompletedTask;

    /// <inheritdoc />
    // TODO: Rename 'Original', 'Initial'?
    public IPEndPoint SourceEndPoint { get; private set; }

    /// <inheritdoc />
    public IPEndPoint BoundEndPoint => (IPEndPoint)this.listener?.LocalEndpoint;

    /// <summary>Gets a value indicating whether the listener is bound or not.</summary>
    public bool IsBound => this.listener?.Server.IsBound ?? false;

    /// <summary>Gets or sets the connection encryptor factory.</summary>
    protected PipelineEncryptorFactory Encryption { get; set; }

    /// <summary>Gets or sets the connection decryptor factory.</summary>
    protected PipelineDecryptorFactory Decryption { get; set; }

    /// <summary>Gets the controller's logger instance.</summary>
    protected ILog Logger => LogManager.GetLogger(this.GetType());

    /// <inheritdoc />
    public void Start() {
      if (this.isRunning == 1) {
        throw new InvalidOperationException("The client listener is already active");
      }

      this.listener = new TcpListener(this.SourceEndPoint);
      SocketConnection.SetRecommendedServerOptions(this.listener.Server);
      this.listener.Start();

      this.ShutdownTask = Task.Run(() => this.AcceptIncomingSocketsAsync())
        .ContinueWith(task => this.OnListenerComplete(task.Exception));
      this.isRunning = 1;
      this.OnListenerStarted();
    }

    /// <inheritdoc />
    public void Stop() => this.OnListenerComplete(null);

    /// <inheritdoc />
    public virtual void Dispose() => this.Stop();

    /// <summary>Called when the listener has started.</summary>
    protected virtual void OnListenerStarted() {
      this.LifecycleStarted?.Invoke(this, new LifecycleEventArgs());
      this.Logger.InfoFormat("Client listener started; listening on {0}", this.BoundEndPoint);
    }

    /// <summary>Called when the listener has stopped.</summary>
    protected virtual void OnListenerStopped() {
      this.LifecycleEnded?.Invoke(this, new LifecycleEventArgs());
      this.Logger.Info("Client listener stopped");
    }

    /// <summary>Called whenever a client must be created from the its connection.</summary>
    protected abstract TClient OnConnectionEstablished(IConnection connection);

    /// <summary>Processes incoming connections until finish.</summary>
    private async Task AcceptIncomingSocketsAsync() {
      Socket socket;
      try {
        socket = await this.listener.AcceptSocketAsync();
      } catch (ObjectDisposedException) {
        return;
      }

      this.OnSocketAccepted(socket);

      if (this.IsBound) {
        await this.AcceptIncomingSocketsAsync();
      }
    }

    private void OnSocketAccepted(Socket socket) {
      var clientAccept = new ClientSocketAcceptEventArgs(socket);
      this.ClientAccept?.Invoke(this, clientAccept);

      if (clientAccept.RejectClient) {
        this.Logger.DebugFormat("Rejecting client connection {0}...", socket.RemoteEndPoint);
        socket.Dispose();
      } else {
        var connection = this.CreateConnectionForSocket(socket);
        this.ClientConnectionEstablished?.Invoke(this, new ClientConnectionEstablishedEventArgs(connection));
        var client = this.OnConnectionEstablished(connection);
        this.ClientConnected?.Invoke(this, new ClientConnectedEventArgs<TClient>(client));
      }
    }

    private void OnListenerComplete(Exception ex) {
      if (ex != null) {
        this.Logger.Error("An unexpected error occured whilst listening for incoming clients", ex);
      }

      if (Interlocked.Exchange(ref this.isRunning, 0) == 1) {
        try {
          this.listener.Stop();
        } catch (ObjectDisposedException) {
        }

        this.OnListenerStopped();
      }
    }

    private IConnection CreateConnectionForSocket(Socket socket) {
      // Ensure that 'TCP.NODELAY' is configured on Windows
      SocketConnection.SetRecommendedClientOptions(socket);

      // Raw sockets themselves are not compatible with pipes
      var socketConnection = SocketConnection.Create(socket);
      var pipe = new PipelinedSocket(
        socketConnection,
        this.Encryption?.Invoke(socketConnection.Output),
        this.Decryption?.Invoke(socketConnection.Input));

      return new DuplexConnection(pipe, this.maxClientPacketSize);
    }
  }
}
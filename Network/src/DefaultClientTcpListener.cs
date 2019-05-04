using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Muwesome.Interfaces;
using Muwesome.Network;
using Pipelines.Sockets.Unofficial;

namespace Muwesome.Network {
  public class DefaultClientTcpListener : IClientTcpListener, IDisposable {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(DefaultClientTcpListener));
    private readonly int maxClientPacketSize;
    private TcpListener listener;
    private int isRunning;

    /// <summary>Initializes a new instance of the <see cref="DefaultClientTcpListener"/> class.</summary>
    public DefaultClientTcpListener(IPEndPoint endPoint, int maxClientPacketSize) {
      this.maxClientPacketSize = maxClientPacketSize;
      this.SourceEndPoint = endPoint;
    }

    /// <inheritdoc />
    public event EventHandler<LifecycleEventArgs> AfterLifecycleStarted;

    /// <inheritdoc />
    public event EventHandler<LifecycleEventArgs> AfterLifecycleEnded;

    /// <inheritdoc />
    public event EventHandler<BeforeClientAcceptEventArgs> BeforeClientAccepted;

    /// <inheritdoc />
    public event EventHandler<ClientConnectedEventArgs> ClientConnected;

    /// <inheritdoc />
    public Task ShutdownTask { get; private set; } = Task.CompletedTask;

    /// <inheritdoc />
    public IPEndPoint SourceEndPoint { get; private set; }

    /// <inheritdoc />
    public IPEndPoint BoundEndPoint => (IPEndPoint)this.listener?.LocalEndpoint;

    /// <summary>Gets a value indicating whether the listener is bound or not.</summary>
    public bool IsBound => this.listener?.Server.IsBound ?? false;

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

      this.AfterLifecycleStarted?.Invoke(this, new LifecycleEventArgs());
      Logger.Info($"Client listener started; listening on {this.BoundEndPoint}");
    }

    /// <inheritdoc />
    public void Stop() => this.OnListenerComplete(null);

    /// <inheritdoc />
    public void Dispose() => this.Stop();

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
      var beforeClientAccept = new BeforeClientAcceptEventArgs(socket);
      this.BeforeClientAccepted?.Invoke(this, beforeClientAccept);

      if (beforeClientAccept.RejectClient) {
        Logger.Debug($"Rejecting client connection {socket.RemoteEndPoint}...");
        socket.Dispose();
      } else {
        var connection = this.CreateConnectionForSocket(socket);
        this.ClientConnected?.Invoke(this, new ClientConnectedEventArgs(connection));
      }
    }

    private void OnListenerComplete(Exception ex) {
      if (ex != null) {
        Logger.Error("An unexpected error occured whilst listening for incoming clients", ex);
      }

      if (Interlocked.Exchange(ref this.isRunning, 0) == 1) {
        try {
          this.listener.Stop();
        } catch (ObjectDisposedException) {
        }

        this.AfterLifecycleEnded?.Invoke(this, new LifecycleEventArgs());
        Logger.Info("Client listener stopped");
      }
    }

    private IConnection CreateConnectionForSocket(Socket socket) {
      // Ensure that 'TCP.NODELAY' is configured on Windows
      SocketConnection.SetRecommendedClientOptions(socket);

      // Raw sockets themselves are not compatible with pipes
      var socketConnection = SocketConnection.Create(socket);
      var pipe = new PipelinedSocket(socketConnection, encryptor: null, decryptor: null);

      return new DuplexConnection(pipe, this.maxClientPacketSize);
    }
  }
}
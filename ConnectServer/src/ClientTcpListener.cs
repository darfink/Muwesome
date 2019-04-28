using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Muwesome.Network;
using Muwesome.Packet.IO;
using Muwesome.Packet.IO.Xor;
using Muwesome.Protocol;
using Pipelines.Sockets.Unofficial;

namespace Muwesome.ConnectServer {
  internal class ClientTcpListener : IClientListener, IDisposable {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(ClientTcpListener));
    private readonly Configuration config;
    private TcpListener listener;

    /// <summary>Initializes a new instance of the <see cref="ClientTcpListener"/> class.</summary>
    public ClientTcpListener(Configuration config) => this.config = config;

    /// <inheritdoc />
    public event EventHandler<BeforeClientAcceptEventArgs> BeforeClientAccepted;

    /// <inheritdoc />
    public event EventHandler<AfterClientAcceptEventArgs> AfterClientAccepted;

    /// <inheritdoc />
    public bool IsBound => this.listener?.Server.IsBound ?? false;

    /// <inheritdoc />
    public Task Task { get; private set; } = Task.CompletedTask;

    /// <inheritdoc />
    public void Start() {
      if (this.listener != null) {
        throw new InvalidOperationException("The client listener is already running");
      }

      var endPoint = new IPEndPoint(this.config.ClientListenerHost, this.config.ClientListenerPort);
      this.listener = new TcpListener(endPoint);
      SocketConnection.SetRecommendedServerOptions(this.listener.Server);
      this.listener.Start();
      Logger.Info($"Client listener started; listening on {endPoint}");
      this.Task = Task
        .Run(() => this.listener.AcceptIncomingSocketsAsync(this.OnSocketAccepted))
        .ContinueWith(task => this.OnListenerComplete(task.Exception));
    }

    /// <inheritdoc />
    public void Stop() => this.OnListenerComplete(null);

    /// <inheritdoc />
    public void Dispose() => this.Stop();

    private void OnSocketAccepted(Socket socket) {
      var beforeClientAccept = new BeforeClientAcceptEventArgs(socket);
      this.BeforeClientAccepted?.Invoke(this, beforeClientAccept);

      if (beforeClientAccept.RejectClient) {
        Logger.Debug($"Rejecting client connection {socket.RemoteEndPoint}...");
        socket.Dispose();
      } else {
        var connection = this.CreateConnectionForSocket(socket);
        this.AfterClientAccepted?.Invoke(this, new AfterClientAcceptEventArgs(connection));
      }
    }

    private void OnListenerComplete(Exception ex) {
      var listener = Interlocked.Exchange(ref this.listener, null);
      if (listener != null) {
        try {
          listener.Stop();
        } catch (ObjectDisposedException) {
        }

        Logger.Info("Client listener stopped");
      }

      if (ex != null) {
        Logger.Error("An unexpected error occured whilst listening for incoming clients", ex);
      }
    }

    private IConnection CreateConnectionForSocket(Socket socket) {
      // Ensure that 'TCPthis.NODELAY' is configured on Windows
      SocketConnection.SetRecommendedClientOptions(socket);

      // Raw sockets themselves are not compatible with pipes
      var socketConnection = SocketConnection.Create(socket);
      var pipe = new PipelinedSocket(socketConnection, encryptor: null, decryptor: null);

      return new DuplexConnection(pipe, this.config.MaxPacketSize);
    }
  }
}
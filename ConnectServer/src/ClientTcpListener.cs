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
    private readonly Configuration _config;
    private TcpListener _listener;

    /// <summary>Creates a new <see cref="ClientTcpListener" />.</summary>
    public ClientTcpListener(Configuration config) => _config = config;

    /// <inheritdoc />
    public event EventHandler<BeforeClientAcceptEventArgs> BeforeClientAccepted;

    /// <inheritdoc />
    public event EventHandler<AfterClientAcceptEventArgs> AfterClientAccepted;

    /// <inheritdoc />
    public bool IsBound => _listener?.Server.IsBound ?? false;

    /// <inheritdoc />
    public Task Task { get; private set; } = Task.CompletedTask;

    /// <inheritdoc />
    public void Start() {
      if (_listener != null) {
        throw new InvalidOperationException("The client listener is already running");
      }

      var endPoint = new IPEndPoint(_config.ClientListenerHost, _config.ClientListenerPort);
      _listener = new TcpListener(endPoint);
      SocketConnection.SetRecommendedServerOptions(_listener.Server);
      _listener.Start();
      Logger.Info($"Client listener started; listening on {endPoint}");
      Task = Task
        .Run(() => _listener.AcceptIncomingSocketsAsync(OnSocketAccepted))
        .ContinueWith(task => OnListenerComplete(task.Exception));
    }

    /// <inheritdoc />
    public void Stop() => OnListenerComplete(null);

    /// <inheritdoc />
    public void Dispose() => Stop();

    private void OnSocketAccepted(Socket socket) {
      var beforeClientAccept = new BeforeClientAcceptEventArgs(socket);
      BeforeClientAccepted?.Invoke(this, beforeClientAccept);

      if (beforeClientAccept.RejectClient) {
        Logger.Debug($"Rejecting client connection {socket.RemoteEndPoint}...");
        socket.Dispose();
      } else {
        var connection = CreateConnectionForSocket(socket);
        AfterClientAccepted?.Invoke(this, new AfterClientAcceptEventArgs(connection));
      }
    }

    private void OnListenerComplete(Exception ex) {
      var listener = Interlocked.Exchange(ref _listener, null);
      if (listener != null) {
        try { listener.Stop(); }
        catch (ObjectDisposedException) { }
        Logger.Info("Client listener stopped");
      }

      if (ex != null) {
        Logger.Error("An unexpected error occured whilst listening for incoming clients", ex);
      }
    }

    private IConnection CreateConnectionForSocket(Socket socket) {
      // Ensure that 'TCP_NODELAY' is configured on Windows
      SocketConnection.SetRecommendedClientOptions(socket);

      // Raw sockets themselves are not compatible with pipes
      var socketConnection = SocketConnection.Create(socket);
      var pipe = new PipelinedSocket(socketConnection, encryptor: null, decryptor: null);

      return new DuplexConnection(pipe, _config.MaxPacketSize);
    }
  }
}
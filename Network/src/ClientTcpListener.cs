using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using log4net;
using Muwesome.Interfaces;
using Muwesome.Network;
using Pipelines.Sockets.Unofficial;

namespace Muwesome.Network {
  public class ClientTcpListener : IClientListener, IDisposable {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(ClientTcpListener));
    private readonly Dictionary<IPEndPoint, TcpListener> listeners;
    private readonly int maxClientPacketSize;

    /// <summary>Initializes a new instance of the <see cref="ClientTcpListener"/> class.</summary>
    public ClientTcpListener(int maxClientPacketSize, params IPEndPoint[] endPoints) {
      this.listeners = endPoints.ToDictionary(l => l, _ => (TcpListener)null);
      this.maxClientPacketSize = maxClientPacketSize;
    }

    /// <inheritdoc />
    public event EventHandler<LifecycleEventArgs> AfterLifecycleStarted;

    /// <inheritdoc />
    public event EventHandler<LifecycleEventArgs> AfterLifecycleEnded;

    /// <inheritdoc />
    public event EventHandler<BeforeClientAcceptEventArgs> BeforeClientAccepted;

    /// <inheritdoc />
    public event EventHandler<AfterClientAcceptEventArgs> AfterClientAccepted;

    /// <inheritdoc />
    public bool IsBound => this.listeners.Values.Any(listener => listener?.Server.IsBound ?? false);

    /// <inheritdoc />
    public Task ShutdownTask { get; private set; } = Task.CompletedTask;

    /// <inheritdoc />
    public void Start() {
      if (this.IsBound) {
        throw new InvalidOperationException("The client listener is already running");
      }

      var tasks = this.listeners.Keys.ToList().Select(endPoint => {
        var listener = this.listeners[endPoint] = new TcpListener(endPoint);
        SocketConnection.SetRecommendedServerOptions(listener.Server);
        listener.Start();
        return Task.Run(() => this.AcceptIncomingSocketsAsync(listener));
      });

      this.ShutdownTask = Task.WhenAll(tasks).ContinueWith(task => this.OnListenerComplete(task.Exception));
      this.AfterLifecycleStarted?.Invoke(this, new LifecycleEventArgs());

      var activeEndPoints = string.Join(", ", this.listeners.Keys);
      Logger.Info($"Client listener started; listening on {activeEndPoints}");
    }

    /// <inheritdoc />
    public void Stop() => this.OnListenerComplete(null);

    /// <inheritdoc />
    public void Dispose() => this.Stop();

    /// <summary>Processes incoming connections until finish.</summary>
    private async Task AcceptIncomingSocketsAsync(TcpListener listener) {
      Socket socket;
      try {
        socket = await listener.AcceptSocketAsync();
      } catch (ObjectDisposedException) {
        return;
      }

      this.OnSocketAccepted((IPEndPoint)listener.LocalEndpoint, socket);

      if (listener.Server.IsBound) {
        await this.AcceptIncomingSocketsAsync(listener);
      }
    }

    private void OnSocketAccepted(IPEndPoint localEndPoint, Socket socket) {
      var beforeClientAccept = new BeforeClientAcceptEventArgs(localEndPoint, socket);
      this.BeforeClientAccepted?.Invoke(this, beforeClientAccept);

      if (beforeClientAccept.RejectClient) {
        Logger.Debug($"Rejecting client connection {socket.RemoteEndPoint}...");
        socket.Dispose();
      } else {
        var connection = this.CreateConnectionForSocket(socket);
        this.AfterClientAccepted?.Invoke(this, new AfterClientAcceptEventArgs(localEndPoint, connection));
      }
    }

    private void OnListenerComplete(Exception ex) {
      if (ex != null) {
        Logger.Error("An unexpected error occured whilst listening for incoming clients", ex);
      }

      if (this.IsBound) {
        try {
          foreach (var listener in this.listeners.Values) {
            listener.Stop();
          }
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
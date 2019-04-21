using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Muwesome.Network;
using Muwesome.Packet.IO;
using Muwesome.Packet.IO.Xor;
using Muwesome.Protocol;
using Pipelines.Sockets.Unofficial;

namespace Muwesome.ConnectServer {
  internal class ClientListener : IClientsController, IDisposable {
    private readonly IPacketHandler<Client> _packetHandler;
    private readonly ConcurrentDictionary<Client, byte> _clients;
    private readonly Configuration _config;
    private TcpListener _listener;

    /// <summary>Constructs a new client listener.</summary>
    public ClientListener(Configuration config) {
      _config = config;
      _clients = new ConcurrentDictionary<Client, byte>();
      _packetHandler = new ClientProtocolHandler(this) {
        DisconnectOnUnknownPacket = _config.DisconnectOnUnknownPacket
      };
    }

    /// <inheritdoc />
    public event EventHandler<BeforeClientAcceptEventArgs> BeforeClientAccepted;

    /// <inheritdoc />
    public event EventHandler<AfterClientAcceptEventArgs> AfterClientAccepted;

    /// <inheritdoc />
    public event EventHandler<AfterClientDisconnectEventArgs> AfterClientDisconnected;

    /// <inheritdoc />
    public IReadOnlyCollection<Client> Clients => _clients.Keys.AsReadOnly();

    /// <summary>Gets whether the listener is bound or not.</summary>
    public bool IsBound => _listener?.Server.IsBound ?? false;

    /// <summary>Starts the client listener.</summary>
    public void Start() {
      if (_listener != null) {
        throw new InvalidOperationException("The client listener is already running");
      }

      var endPoint = new IPEndPoint(_config.ClientListenerHost, _config.ClientListenerPort);
      _listener = new TcpListener(endPoint);
      SocketConnection.SetRecommendedServerOptions(_listener.Server);
      _listener.Start();
      Task
        .Run(() => _listener.AcceptIncomingSocketsAsync(OnSocketAccepted))
        .ContinueWith(task => OnListenerComplete(task.Exception));
    }

    /// <summary>Stops the client listener.</summary>
    public void Stop() => OnListenerComplete(null);

    /// <inheritdoc />
    public void Dispose() {
      Stop();
      foreach (Client client in _clients.Keys.ToList()) {
        client.Dispose();
      }
    }

    private void OnSocketAccepted(Socket socket) {
      var beforeClientAccept = new BeforeClientAcceptEventArgs(socket);
      BeforeClientAccepted?.Invoke(this, beforeClientAccept);

      if (beforeClientAccept.RejectClient) {
        socket.Dispose();
        return;
      }

      AddClient(socket);
    }

    private void OnClientDisconnected(Client client) {
      // TODO: LOOGGG DISZZZ SHIT
      Debug.Assert(_clients.TryRemove(client, out _));
      using (client) {
        AfterClientDisconnected?.Invoke(this, new AfterClientDisconnectEventArgs(client));
      }
    }

    private void OnListenerComplete(Exception ex) {
      var listener = Interlocked.Exchange(ref _listener, null);
      if (listener != null) {
        try { listener.Stop(); }
        catch (ObjectDisposedException) { }
        // TODO: LOGGG HERRREEE!!!
      }
    }

    private void AddClient(Socket socket) {
      var client = new Client(CreateConnectionForSocket(socket), _packetHandler);
      client.MaxIdleTime = _config.MaxIdleTime;
      Debug.Assert(_clients.TryAdd(client, 0));
      client.Connection.Disconnected += (_, __) => OnClientDisconnected(client);
      AfterClientAccepted?.Invoke(this, new AfterClientAcceptEventArgs(client));
    }

    private IConnection CreateConnectionForSocket(Socket socket) {
      // Ensure that 'TCP_NODELAY' is configured on Windows
      SocketConnection.SetRecommendedClientOptions(socket);

      // Sockets themselves are not compatible with pipes
      var socketConnection = SocketConnection.Create(socket);
      var pipe = new PipelinedSocket(socketConnection, encryptor: null, decryptor: null);

      return new DuplexConnection(pipe, _config.MaxPacketSize);
    }
  }
}
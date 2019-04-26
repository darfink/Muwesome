using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using log4net;
using Muwesome.ConnectServer.Utility;

namespace Muwesome.ConnectServer {
  public class ClientController : IClientController, IDisposable {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(ClientController));
    private readonly ConcurrentDictionary<Client, byte> _clients;
    private readonly Configuration _config;

    /// <summary>Creates a new <see cref="ClientController" />.</summary>
    public ClientController(Configuration config) {
      _clients = new ConcurrentDictionary<Client, byte>();
      _config = config;
    }

    /// <inheritdoc />
    public event EventHandler<ClientSessionEventArgs> ClientSessionStarted;

    /// <inheritdoc />
    public event EventHandler<ClientSessionEventArgs> ClientSessionEnded;

    /// <inheritdoc />
    public IReadOnlyCollection<Client> Clients => _clients.Keys.AsReadOnly();

    /// <inheritdoc />
    public void AddClient(Client client) {
      client.MaxIdleTime = _config.MaxIdleTime;
      Debug.Assert(_clients.TryAdd(client, 0));
      client.Connection.Disconnected += (_, __) => OnClientDisconnected(client);
      ClientSessionStarted?.Invoke(this, new ClientSessionEventArgs(client));
      Logger.Info($"Client connected {client}; current client count {_clients.Count}");
    }

    /// <inheritdoc />
    public void Dispose() {
      foreach (Client client in _clients.Keys.ToList()) {
        client.Dispose();
      }
    }

    private void OnClientDisconnected(Client client) {
      Logger.Info($"Client disconnected {client}");
      Debug.Assert(_clients.TryRemove(client, out _));
      using (client) {
        ClientSessionEnded?.Invoke(this, new ClientSessionEventArgs(client));
      }
    }
  }
}
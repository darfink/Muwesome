using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using log4net;
using Muwesome.GameLogic;

namespace Muwesome.GameServer {
  /// <summary>A controller for clients.</summary>
  internal class ClientController : IClientController, IDisposable {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(ClientController));
    private readonly ConcurrentDictionary<Player, Client> clients;
    private readonly TimeSpan maxClientIdleTime;

    /// <summary>Initializes a new instance of the <see cref="ClientController"/> class.</summary>
    public ClientController(TimeSpan maxClientIdleTime) {
      this.clients = new ConcurrentDictionary<Player, Client>();
      this.maxClientIdleTime = maxClientIdleTime;
    }

    /// <inheritdoc />
    public event EventHandler<ClientSessionEventArgs> ClientSessionStarted;

    /// <inheritdoc />
    public event EventHandler<ClientSessionEventArgs> ClientSessionEnded;

    /// <inheritdoc />
    public int ClientsConnected => this.clients.Count;

    /// <inheritdoc />
    public void AddClient(Client client) {
      Debug.Assert(this.clients.TryAdd(client.Player, client), "Client could not be added");

      client.MaxIdleTime = this.maxClientIdleTime;
      client.Connection.Disconnected += (_, ev) => this.OnClientDisconnected(client);
      client.Player.Disposed += (_, ev) => client.Connection.Disconnect();

      this.ClientSessionStarted?.Invoke(this, new ClientSessionEventArgs(client));
      Logger.Info($"Client connected {client}; current client count {this.clients.Count}");
    }

    /// <inheritdoc />
    public Client GetClientByPlayer(Player player) => this.clients[player];

    /// <inheritdoc />
    public void Dispose() {
      foreach (Client client in this.clients.Values.ToList()) {
        client.Dispose();
      }
    }

    private void OnClientDisconnected(Client client) {
      Logger.Info($"Client disconnected {client}");
      Debug.Assert(this.clients.TryRemove(client.Player, out _), "Client could not be removed");
      using (client) {
        this.ClientSessionEnded?.Invoke(this, new ClientSessionEventArgs(client));
      }
    }
  }
}
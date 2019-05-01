using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using log4net;
using Muwesome.ConnectServer.Utility;

namespace Muwesome.ConnectServer {
  internal class ClientController : IClientController, IDisposable {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(ClientController));
    private readonly ConcurrentDictionary<Client, byte> clients;
    private readonly TimeSpan maxClientIdleTime;

    /// <summary>Initializes a new instance of the <see cref="ClientController"/> class.</summary>
    public ClientController(TimeSpan maxClientIdleTime) {
      this.clients = new ConcurrentDictionary<Client, byte>();
      this.maxClientIdleTime = maxClientIdleTime;
    }

    /// <inheritdoc />
    public event EventHandler<ClientSessionEventArgs> ClientSessionStarted;

    /// <inheritdoc />
    public event EventHandler<ClientSessionEventArgs> ClientSessionEnded;

    /// <inheritdoc />
    public IReadOnlyCollection<Client> Clients => this.clients.Keys.AsReadOnly();

    /// <inheritdoc />
    public void AddClient(Client client) {
      client.MaxIdleTime = this.maxClientIdleTime;
      Debug.Assert(this.clients.TryAdd(client, 0), "Client could not be added");
      client.Connection.Disconnected += (_, ev) => this.OnClientDisconnected(client);
      this.ClientSessionStarted?.Invoke(this, new ClientSessionEventArgs(client));
      Logger.Info($"Client connected {client}; current client count {this.clients.Count}");
    }

    /// <inheritdoc />
    public void Dispose() {
      foreach (Client client in this.clients.Keys.ToList()) {
        client.Dispose();
      }
    }

    private void OnClientDisconnected(Client client) {
      Logger.Info($"Client disconnected {client}");
      Debug.Assert(this.clients.TryRemove(client, out _), "Client could not be removed");
      using (client) {
        this.ClientSessionEnded?.Invoke(this, new ClientSessionEventArgs(client));
      }
    }
  }
}
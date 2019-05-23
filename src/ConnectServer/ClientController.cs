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

    /// <summary>Initializes a new instance of the <see cref="ClientController"/> class.</summary>
    public ClientController() => this.clients = new ConcurrentDictionary<Client, byte>();

    /// <inheritdoc />
    public event EventHandler<ClientSessionEventArgs> ClientSessionStarted;

    /// <inheritdoc />
    public event EventHandler<ClientSessionEventArgs> ClientSessionEnded;

    /// <inheritdoc />
    public int ClientsConnected => this.clients.Count;

    /// <inheritdoc />
    public void AddClient(Client client) {
      Debug.Assert(this.clients.TryAdd(client, 0), "Client could not be added");
      client.Connection.Disconnected += (_, ev) => this.OnClientDisconnected(client);

      this.ClientSessionStarted?.Invoke(this, new ClientSessionEventArgs(client));
      Logger.InfoFormat("Client connected {0}; current client count {1}", client, this.clients.Count);
    }

    /// <inheritdoc />
    public void Dispose() {
      foreach (Client client in this.clients.Keys.ToList()) {
        client.Dispose();
      }
    }

    private void OnClientDisconnected(Client client) {
      Logger.InfoFormat("Client disconnected {0}", client);
      Debug.Assert(this.clients.TryRemove(client, out _), "Client could not be removed");
      using (client) {
        this.ClientSessionEnded?.Invoke(this, new ClientSessionEventArgs(client));
      }
    }
  }
}
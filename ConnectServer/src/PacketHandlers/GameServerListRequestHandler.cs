using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Muwesome.Network;
using Muwesome.Packet.Utility;
using Muwesome.Protocol;
using Muwesome.Protocol.Connect.V20050502;

namespace Muwesome.ConnectServer.PacketHandlers {
  internal class GameServerListRequestHandler : IPacketHandler<Client> {
    private readonly IGameServerController gameServerController;
    private readonly ConcurrentBag<Client> impairedClients = new ConcurrentBag<Client>();
    private byte[] gameServerListPacket = Array.Empty<byte>();
    private int gameServerListPacketSize;
    private int gameServersInResponse;

    /// <summary>Initializes a new instance of the <see cref="GameServerListRequestHandler"/> class.</summary>
    public GameServerListRequestHandler(IGameServerController gameServerController) {
      this.gameServerController = gameServerController;
      this.gameServerController.GameServerRegistered += (_, ev) => this.OnGameServerChange();
      this.gameServerController.GameServerDeregistered += (_, ev) => this.OnGameServerChange();
      this.gameServerController.GameServerUpdated += (_, ev) => this.OnGameServerChange();
    }

    /// <inheritdoc />
    public bool HandlePacket(Client client, Span<byte> packet) {
      if (this.gameServerListPacketSize == 0) {
        this.CreateServerListPacket(this.gameServerController.Servers);
      }

      using (var writer = client.Connection.StartWrite(this.gameServerListPacketSize)) {
        this.gameServerListPacket.AsSpan().Slice(0, this.gameServerListPacketSize).CopyTo(writer.Span);
      }

      if (this.gameServersInResponse == 0) {
        this.impairedClients.Add(client);
      }

      return true;
    }

    private void CreateServerListPacket(IReadOnlyCollection<GameServerEntry> servers) {
      this.gameServerListPacketSize = PacketHelper.GetPacketSize<GameServerList, GameServerList.GameServer>(servers.Count);

      if (this.gameServerListPacket.Length < this.gameServerListPacketSize) {
        this.gameServerListPacket = new byte[this.gameServerListPacketSize];
      }

      this.gameServersInResponse = servers.Count;
      PacketHelper.CreatePacket<GameServerList, GameServerList.GameServer>(
        servers.Count,
        this.gameServerListPacket.AsSpan(),
        out Span<GameServerList.GameServer> serverEntries);

      foreach (var (index, server) in servers.Select((s, i) => (i, s))) {
        serverEntries[index].Code = server.Code;
        serverEntries[index].Load = server.Load;
      }
    }

    private void OnGameServerChange() {
      this.gameServerListPacketSize = 0;
      this.FreeImpairedClients();
    }

    /// <summary>Frees any impaired game clients.</summary>
    /// <remarks>
    /// If the client receives an empty server list response, it becomes stuck
    /// due to the user not being able to issue new requests, forcing them to
    /// restart the client. To solve this, a new server list response is sent to
    /// all clients that have previously received any empty response.
    /// </remarks>
    private void FreeImpairedClients() {
      if (this.impairedClients.IsEmpty) {
        return;
      }

      if (this.gameServerListPacketSize == 0) {
        this.CreateServerListPacket(this.gameServerController.Servers);
      }

      if (this.gameServersInResponse == 0) {
        return;
      }

      while (this.impairedClients.TryTake(out Client client)) {
        if (!client.Connection.IsConnected) {
          continue;
        }

        using (var writer = client.Connection.StartWrite(this.gameServerListPacketSize)) {
          this.gameServerListPacket.AsSpan().Slice(0, this.gameServerListPacketSize).CopyTo(writer.Span);
        }
      }
    }
  }
}
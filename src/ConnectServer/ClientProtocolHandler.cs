using System;
using System.Threading.Tasks;
using log4net;
using Muwesome.ConnectServer.PacketHandlers;
using Muwesome.Network;
using Muwesome.Packet;
using Muwesome.Packet.Utility;
using Muwesome.Protocol;
using Muwesome.Protocol.Connect.Client;
using Muwesome.Protocol.Connect.Server;

namespace Muwesome.ConnectServer {
  internal class ClientProtocolHandler : ConfigurablePacketHandler<Client> {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(ClientProtocolHandler));

    /// <summary>Initializes a new instance of the <see cref="ClientProtocolHandler" /> class.</summary>
    public ClientProtocolHandler(
        IGameServerController gameServerController,
        IClientController clientsController) {
      clientsController.ClientSessionStarted += this.OnClientSessionStarted;
      this.RegisterPacketHandlers(gameServerController);
    }

    /// <summary>Gets or sets a value indicating whether client's are disconnected when sending unknown packets.</summary>
    public bool DisconnectOnUnknownPacket { get; set; }

    /// <inheritdoc />
    public override bool HandlePacket(Client client, Span<byte> packet) {
      bool packetWasHandled = base.HandlePacket(client, packet);

      if (!packetWasHandled) {
        Logger.Debug($"Received an unhandled packet: {packet.ToHexString()}");
        if (this.DisconnectOnUnknownPacket) {
          Logger.Info($"Disconnecting client {client}; received an unknown packet");
          client.Connection.Disconnect();
        }
      }

      return packetWasHandled;
    }

    private void RegisterPacketHandlers(IGameServerController gameServerController) {
      this.Register<GameServerInfoRequest>(new GameServerInfoRequestHandler(gameServerController));
      this.Register<GameServerListRequest>(new GameServerListRequestHandler(gameServerController));
      this.Register<ClientUpdateRequest>(new ClientUpdateRequestHandler());
    }

    private void OnClientSessionStarted(object sender, ClientSessionEventArgs ev) {
      using (var writer = ev.Client.Connection.SendPacket<ConnectResult>()) {
        writer.Packet.Success = true;
      }
    }
  }
}
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
        Logger.DebugFormat("Received an unhandled packet: {0}", packet.ToHexString());
        if (this.DisconnectOnUnknownPacket) {
          Logger.InfoFormat("Disconnecting client {0}; received an unknown packet", client);
          client.Connection.Disconnect();
        }
      }

      return packetWasHandled;
    }

    private void RegisterPacketHandlers(IGameServerController gameServerController) {
      this.RegisterHandler<GameServerInfoRequest>(new GameServerInfoRequestHandler(gameServerController));
      this.RegisterHandler<GameServerListRequest>(new GameServerListRequestHandler(gameServerController));
      this.RegisterHandler<ClientUpdateRequest>(new ClientUpdateRequestHandler());
    }

    private void OnClientSessionStarted(object sender, ClientSessionEventArgs ev) {
      using (var writer = ev.Client.Connection.SendPacket<ConnectResult>()) {
        writer.Packet.Success = true;
      }
    }
  }
}
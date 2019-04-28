using System;
using System.Threading.Tasks;
using log4net;
using Muwesome.ConnectServer.PacketHandlers;
using Muwesome.Network;
using Muwesome.Packet;
using Muwesome.Protocol.Connect.V20050502;
using Muwesome.Protocol;

namespace Muwesome.ConnectServer {
  internal class ClientProtocolHandler : ConfigurablePacketHandler<Client> {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(ClientProtocolHandler));

    /// <summary>Creates a new <see cref="ClientProtocolHandler" />.</summary>
    public ClientProtocolHandler(
        Configuration config,
        IGameServerController gameServerController,
        IClientController clientsController) {
      clientsController.ClientSessionStarted += OnClientSessionStarted;
      DisconnectOnUnknownPacket = config.DisconnectOnUnknownPacket;
      RegisterPacketHandlers(gameServerController);
    }

    /// <summary>Gets or sets whether client's are disconnected when sending unknown packets.</summary>
    public bool DisconnectOnUnknownPacket { get; set; }

    /// <inheritdoc />
    public override bool HandlePacket(Client client, Span<byte> packet) {
      bool packetWasHandled = base.HandlePacket(client, packet);

      if (!packetWasHandled) {
        Logger.Debug($"Received an unhandled packet: {packet.AsHexString()}");
        if (DisconnectOnUnknownPacket) {
          Logger.Info($"Disconnecting client {client}; received an unknown packet");
          client.Connection.Disconnect();
        }
      }

      return packetWasHandled;
    }

    private void RegisterPacketHandlers(IGameServerController gameServerController) {
      Register<GameServerInfoRequest>(new GameServerInfoRequestHandler(gameServerController));
      Register<GameServerListRequest>(new GameServerListRequestHandler(gameServerController));
      Register<ClientUpdateRequest>(new ClientUpdateRequestHandler());
    }

    private void OnClientSessionStarted(object sender, ClientSessionEventArgs ev) {
      var size = ProtocolHelper.GetPacketSize<ConnectResult>();

      using (var writer = ev.Client.Connection.StartWrite(size)) {
        ref var result = ref ProtocolHelper.CreatePacket<ConnectResult>(writer.Span);
        result.Success = true;
      }
    }
  }
}
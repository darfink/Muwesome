using System;
using log4net;
using Muwesome.Packet;
using Muwesome.Network;
using Muwesome.Protocol;
using Muwesome.Protocol.Connect.V20050502;

namespace Muwesome.ConnectServer {
  internal class ClientProtocolHandler : ConfigurablePacketHandler<Client> {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(ClientProtocolHandler));

    public ClientProtocolHandler(Configuration config, IClientController clientsController) {
      // TODO: Register dem handlers
      clientsController.ClientSessionStarted += OnClientSessionStarted;
      DisconnectOnUnknownPacket = config.DisconnectOnUnknownPacket;
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

    private void OnClientSessionStarted(object sender, ClientSessionEventArgs ev) {
      using(var writer = ev.Client.Connection.StartWrite(ConnectResult.Size)) {
        var result = ConnectResult.Create(writer.Span);
        result.Success = true;
      }
    }
  }
}
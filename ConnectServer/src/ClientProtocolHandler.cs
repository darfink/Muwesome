using System;
using Muwesome.Network;
using Muwesome.Protocol;
using Muwesome.Protocol.Connect.V20050502;

namespace Muwesome.ConnectServer {
  internal class ClientProtocolHandler : ConfigurablePacketHandler<Client> {
    public ClientProtocolHandler(IClientsController clientsController) {
      clientsController.AfterClientAccepted += OnAfterClientAccepted;
    }

    /// <summary>Gets or sets whether client's are disconnected when sending unknown packets.</summary>
    public bool DisconnectOnUnknownPacket { get; set; }

    /// <inheritdoc />
    public override bool HandlePacket(Client client, Span<byte> packet) {
      bool packetWasHandled = base.HandlePacket(client, packet);

      if (!packetWasHandled) {
        // TODO: LOOOG HERRREEEE!!!!!
        if (DisconnectOnUnknownPacket) {
          // TODO: AND LOOGG GHEERERE
          client.Connection.Disconnect();
        }
      }

      return packetWasHandled;
    }

    private void OnAfterClientAccepted(object sender, AfterClientAcceptEventArgs ev) {
      using(var writer = ev.AcceptedClient.Connection.SendPacket<ConnectResult>()) {
        var result = ConnectResult.From(writer.Span);
        result.Success = true;
      }
    }
  }
}
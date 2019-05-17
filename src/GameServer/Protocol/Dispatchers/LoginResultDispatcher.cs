using Muwesome.GameLogic;
using Muwesome.GameLogic.Actions.Players;
using Muwesome.Network;

namespace Muwesome.GameServer.Protocol.Dispatchers {
  using LoginResultPacket = Muwesome.Protocol.Game.Server.LoginResult;

  /// <summary>A packet dispatcher for login results.</summary>
  internal class LoginResultDispatcher : PacketDispatcher<LoginResultPacket, ShowLoginResultAction> {
    /// <inheritdoc />
    protected override ShowLoginResultAction CreateDispatcherForAction(Client client) => (result) => this.SendLoginResult(client, result);

    /// <summary>Sends the login result to a client.</summary>
    private void SendLoginResult(Client client, LoginResult result) {
      using (var writer = client.Connection.SendPacket<LoginResultPacket>()) {
        writer.Packet.Result = (byte)result;
      }
    }
  }
}
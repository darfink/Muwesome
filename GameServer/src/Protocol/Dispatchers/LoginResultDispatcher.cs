using Muwesome.GameLogic;
using Muwesome.GameLogic.Actions;
using Muwesome.Network;

namespace Muwesome.GameServer.Protocol.Dispatchers {
  /// <summary>A packet dispatcher for login results.</summary>
  internal class LoginResultDispatcher : PacketDispatcher<ShowLoginResultAction> {
    /// <summary>Initializes a new instance of the <see cref="LoginResultDispatcher"/> class.</summary>
    public LoginResultDispatcher(IClientController clientController)
        : base(clientController) {
    }

    /// <inheritdoc />
    protected override ShowLoginResultAction CreateAction(Client client) =>
      (result) => this.SendLoginResult(client, result);

    /// <summary>Sends the login result to a client.</summary>
    private void SendLoginResult(Client client, LoginResult result) {
      using (var writer = client.Connection.SendPacket<Muwesome.Protocol.Game.LoginResult>()) {
        writer.Packet.Result = (byte)result;
      }
    }
  }
}
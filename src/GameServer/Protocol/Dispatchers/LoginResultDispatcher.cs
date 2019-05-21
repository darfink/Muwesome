using System.Linq;
using System.Linq.Expressions;
using Muwesome.GameLogic;
using Muwesome.GameLogic.Actions.Players;
using Muwesome.MethodDelegate;
using Muwesome.Network;

namespace Muwesome.GameServer.Protocol.Dispatchers {
  using LoginResultPacket = Muwesome.Protocol.Game.Server.LoginResult;

  /// <summary>A packet dispatcher for login results.</summary>
  internal class LoginResultDispatcher : PacketDispatcher<LoginResultPacket> {
    /// <summary>Sends the login result to a client.</summary>
    [MethodDelegate(typeof(ShowLoginResultAction))]
    public void SendLoginResult([Inject] Client client, LoginResult result) {
      using (var writer = client.Connection.SendPacket<LoginResultPacket>()) {
        writer.Packet.Result = (byte)result;
      }
    }
  }
}
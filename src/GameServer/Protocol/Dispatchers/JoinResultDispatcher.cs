using Muwesome.GameLogic;
using Muwesome.GameLogic.Actions.Players;
using Muwesome.MethodDelegate;
using Muwesome.Network;
using Muwesome.Protocol.Game.Server;

namespace Muwesome.GameServer.Protocol.Dispatchers {
  /// <summary>A packet dispatcher for join results.</summary>
  internal class JoinResultDispatcher : PacketDispatcher<JoinResult> {
    /// <summary>Sends the join result to a client.</summary>
    [MethodDelegate(typeof(ShowLoginWindowAction))]
    public void SendJoinResult([Inject] Client client) {
      using (var writer = client.Connection.SendPacket<JoinResult>()) {
        writer.Packet = new JoinResult {
          Success = true,
          PlayerId = 0x1337, // TODO: Magic constant
          Version = client.Version,
        };
      }
    }
  }
}
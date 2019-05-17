using Muwesome.GameLogic;
using Muwesome.GameLogic.Actions.Players;
using Muwesome.Network;
using Muwesome.Protocol.Game.Server;

namespace Muwesome.GameServer.Protocol.Dispatchers {
  /// <summary>A packet dispatcher for join results.</summary>
  internal class JoinResultDispatcher : PacketDispatcher<JoinResult, ShowLoginWindowAction> {
    /// <inheritdoc />
    protected override ShowLoginWindowAction CreateDispatcherForAction(Client client) => () => this.SendJoinResult(client, true);

    /// <summary>Sends the join result to a client.</summary>
    private void SendJoinResult(Client client, bool success) {
      using (var writer = client.Connection.SendPacket<JoinResult>()) {
        writer.Packet = new JoinResult {
          Success = success,
          PlayerId = 0x1337, // TODO: Magic constant
          Version = client.Version,
        };
      }
    }
  }
}
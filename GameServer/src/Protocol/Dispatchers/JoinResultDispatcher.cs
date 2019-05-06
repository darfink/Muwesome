using Muwesome.GameLogic;
using Muwesome.GameLogic.Actions;
using Muwesome.Network;
using Muwesome.Protocol.Game;

namespace Muwesome.GameServer.Protocol.Dispatchers {
  /// <summary>A packet dispatcher for join results.</summary>
  internal class JoinResultDispatcher : IPlayerActionFactory<ShowLoginWindowAction> {
    private readonly IClientController clientController;

    /// <summary>Initializes a new instance of the <see cref="JoinResultDispatcher"/> class.</summary>
    public JoinResultDispatcher(IClientController clientController) =>
      this.clientController = clientController;

    /// <inheritdoc />
    public ShowLoginWindowAction CreateAction(Player player) {
      var client = this.clientController.GetClientByPlayer(player);
      return () => this.SendJoinResult(client, true);
    }

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
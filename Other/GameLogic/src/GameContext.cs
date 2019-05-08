using System.Collections.Generic;
using Muwesome.GameLogic.Actions;
using Muwesome.GameLogic.Actions.Handlers;

namespace Muwesome.GameLogic {
  /// <summary>A game's context.</summary>
  public class GameContext {
    /// <summary>Gets the game's players.</summary>
    public IList<Player> Players { get; } = new List<Player>();

    /// <summary>Adds a player to the game.</summary>
    public void AddPlayer(Player player) {
      this.Players.Add(player);
      player.RegisterActions((new LoginActionHandler() as IPlayerActionProvider<LoginAction>).AsGeneric());
      player.Disposed += (_, ev) => this.Players.Remove(player);
      player.Action<ShowLoginWindowAction>()?.Invoke();
    }
  }
}
using System.Collections.Generic;

namespace Muwesome.GameLogic {
  /// <summary>A game's context.</summary>
  public class GameContext {
    /// <summary>Gets the game's players.</summary>
    public IList<Player> Players { get; } = new List<Player>();
  }
}
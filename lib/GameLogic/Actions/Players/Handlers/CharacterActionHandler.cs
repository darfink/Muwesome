using System;
using log4net;

namespace Muwesome.GameLogic.Actions.Players.Handlers {
  /// <summary>A character action handler.</summary>
  internal class CharacterActionHandler : IActionProvider<Player, RequestCharactersAction> {
    /// <inheritdoc />
    RequestCharactersAction IActionProvider<Player, RequestCharactersAction>.CreateAction(Player player) =>
      () => player.Action<ShowCharactersAction>()?.Invoke();
  }
}
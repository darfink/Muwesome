using System;
using log4net;
using Muwesome.MethodDelegate;

namespace Muwesome.GameLogic.Actions.Players.Handlers {
  /// <summary>A character action handler.</summary>
  internal class CharacterActionHandler {
    /// <summary>Handles a character list request.</summary>
    [MethodDelegate(typeof(RequestCharactersAction))]
    public void RequestCharacters([Inject] Player player) {
      player.Action<ShowCharactersAction>()?.Invoke();
    }
  }
}
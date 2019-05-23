using System;
using log4net;
using Muwesome.GameLogic.PlayerActions;
using Muwesome.MethodDelegate;

namespace Muwesome.GameLogic.PlayerHandlers {
  /// <summary>A player character handler.</summary>
  internal class CharacterHandler {
    /// <summary>Handles a character list request.</summary>
    [MethodDelegate(typeof(RequestCharactersAction))]
    public void RequestCharacters([Inject] Player player) {
      player.Action<ShowCharactersAction>()?.Invoke();
    }
  }
}
using System;
using log4net;
using Muwesome.GameLogic.Interface.Actions;
using Muwesome.GameLogic.Interface.Events;
using Muwesome.MethodDelegate;

namespace Muwesome.GameLogic.EventHandlers {
  /// <summary>A player character handler.</summary>
  internal class CharacterHandler {
    /// <summary>Handles a character list request.</summary>
    [MethodDelegate(typeof(OnRequestCharacters))]
    public void RequestCharacters(Player player) {
      player.Action<ShowCharacters>()?.Invoke();
    }
  }
}
using System;
using System.Linq;
using System.Reflection;

namespace Muwesome.GameLogic.Interface.Actions {
  /// <summary>Displays the player's login window.</summary>
  public delegate void ShowLoginWindow();

  /// <summary>Displays the login result to the player.</summary>
  public delegate void ShowLoginResult(LoginResult result);

  /// <summary>Displays a message to the player.</summary>
  public delegate void ShowMessage(string message);

  /// <summary>Displays the character list to the player.</summary>
  public delegate void ShowCharacters();

  internal static class PlayerActions {
    /// <summary>The defined player action types.</summary>
    public static readonly Type[] Types =
      typeof(PlayerActions).Assembly
        .GetTypes()
        .Where(t => t.Namespace == typeof(PlayerActions).Namespace && typeof(Delegate).IsAssignableFrom(t))
        .ToArray();
  }
}
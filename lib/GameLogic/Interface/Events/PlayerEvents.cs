using System;
using System.Linq;
using System.Reflection;

namespace Muwesome.GameLogic.Interface.Events {
  /// <summary>Logs the player into the game.</summary>
  public delegate void OnLogin(Player player, string username, string password);

  /// <summary>Logs the player out of the game.</summary>
  public delegate void OnLogout(Player player, LogoutType logoutType);

  /// <summary>Requests the player's character list.</summary>
  public delegate void OnRequestCharacters(Player player);

  internal static class PlayerEvents {
    /// <summary>The defined player event types.</summary>
    public static readonly Type[] Types =
      typeof(PlayerEvents).Assembly
        .GetTypes()
        .Where(t => t.Namespace == typeof(PlayerEvents).Namespace && typeof(Delegate).IsAssignableFrom(t))
        .ToArray();
  }
}
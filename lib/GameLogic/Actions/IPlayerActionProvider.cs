using System;

namespace Muwesome.GameLogic.Actions {
  /// <summary>Represents a generic player action provider.</summary>
  public interface IPlayerActionProvider {
    /// <summary>Creates an generic action delegate for the player.</summary>
    Delegate CreateAction(Player player);
  }

  /// <summary>Represents a player action provider.</summary>
  public interface IPlayerActionProvider<out TAction>
      where TAction : Delegate {
    /// <summary>Creates an action delegate for the player.</summary>
    TAction CreateAction(Player player);
  }

  public static class PlayerActionProviderExtensions {
    public static IPlayerActionProvider AsGeneric<TAction>(this IPlayerActionProvider<TAction> playerActionProvider)
        where TAction : Delegate =>
      new PlayerProviderWrapper(player => playerActionProvider.CreateAction(player));

    private class PlayerProviderWrapper : IPlayerActionProvider {
      private readonly Func<Player, Delegate> provider;

      public PlayerProviderWrapper(Func<Player, Delegate> provider) => this.provider = provider;

      /// <inheritdoc />
      public Delegate CreateAction(Player player) => this.provider.Invoke(player);
    }
  }
}
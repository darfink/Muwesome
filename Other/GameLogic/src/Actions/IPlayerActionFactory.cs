using System;

namespace Muwesome.GameLogic.Actions {
  /// <summary>Represents a generic player action factory.</summary>
  public interface IPlayerActionFactory {
    /// <summary>Creates an generic action delegate for the player.</summary>
    Delegate CreateAction(Player player);
  }

  /// <summary>Represents a player action factory.</summary>
  public interface IPlayerActionFactory<out TAction> where TAction : Delegate {
    /// <summary>Creates an action delegate for the player.</summary>
    TAction CreateAction(Player player);
  }

  public static class PlayerActionFactoryExtensions {
    public static IPlayerActionFactory AsGeneric<TAction>(this IPlayerActionFactory<TAction> playerActionFactory) where TAction : Delegate =>
      new PlayerFactoryWrapper(player => playerActionFactory.CreateAction(player));

    private class PlayerFactoryWrapper : IPlayerActionFactory {
      private Func<Player, Delegate> Factory { get; set; }

      public PlayerFactoryWrapper(Func<Player, Delegate> factory) => this.Factory = factory;

      /// <inheritdoc />
      public Delegate CreateAction(Player player) => Factory.Invoke(player);
    }
  }
}
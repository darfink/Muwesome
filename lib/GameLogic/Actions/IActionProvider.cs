using System;

namespace Muwesome.GameLogic.Actions {
  /// <summary>Represents an action provider.</summary>
  public interface IActionProvider<in TContext> {
    /// <summary>Creates an action associated with a context.</summary>
    Delegate CreateAction(TContext context);
  }

  /// <summary>Represents an action provider.</summary>
  public interface IActionProvider<in TContext, out TAction>
      where TAction : Delegate {
    /// <summary>Creates an action associated with a context.</summary>
    TAction CreateAction(TContext context);
  }
}
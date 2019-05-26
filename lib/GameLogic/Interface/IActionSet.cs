using System;

namespace Muwesome.GameLogic.Interface {
  /// <summary>Represents an action set.</summary>
  public interface IActionSet {
    /// <summary>Gets an invokable action.</summary>
    T Get<T>()
      where T : Delegate;
  }
}
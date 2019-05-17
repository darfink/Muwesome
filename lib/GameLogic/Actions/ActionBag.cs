using System;
using System.Collections.Generic;

namespace Muwesome.GameLogic.Actions {
  /// <summary>Represents a collection of defined actions.</summary>
  internal class ActionBag {
    private readonly IReadOnlyDictionary<Type, Delegate> actions;

    /// <summary>Initializes a new instance of the <see cref="ActionBag"/> class.</summary>
    internal ActionBag(IReadOnlyDictionary<Type, Delegate> actions) => this.actions = actions;

    /// <summary>Gets an invokable action.</summary>
    public T Get<T>()
        where T : Delegate {
      this.actions.TryGetValue(typeof(T), out Delegate result);
      return result as T;
    }
  }
}
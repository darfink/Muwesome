using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Muwesome.GameLogic.Interface;

namespace Muwesome.GameLogic.Utility {
  /// <summary>Represents a collection of defined actions.</summary>
  internal class ActionSet : IActionSet {
    private readonly IReadOnlyDictionary<Type, Delegate> actions;

    /// <summary>Initializes a new instance of the <see cref="ActionSet"/> class.</summary>
    internal ActionSet(IReadOnlyDictionary<Type, Delegate> actions) => this.actions = actions;

    /// <summary>Gets an invokable action.</summary>
    public T Get<T>()
        where T : Delegate {
      this.actions.TryGetValue(typeof(T), out Delegate result);
      return result as T;
    }

    /// <summary>An action set builder.</summary>
    internal class Builder {
      private static readonly ILog Logger = LogManager.GetLogger(typeof(ActionSetDefinition));
      private readonly Dictionary<Type, Delegate> registry = new Dictionary<Type, Delegate>();
      private readonly ISet<Type> definedActions = new HashSet<Type>();

      /// <summary>Initializes a new instance of the <see cref="Builder"/> class.</summary>
      public Builder(ISet<Type> definedActions) => this.definedActions = definedActions;

      /// <summary>Registers an action.</summary>
      public void RegisterAction(Delegate action) {
        var actionType = action.GetType();

        if (!this.definedActions.Contains(actionType)) {
          throw new ArgumentException($"Undefined action type {actionType.Name}");
        }

        if (this.registry.ContainsKey(actionType)) {
          throw new ArgumentException($"Duplicated action type {actionType.Name}");
        }

        this.registry[actionType] = action;
      }

      /// <summary>Builds an action set from the registered actions.</summary>
      public ActionSet Build() {
        foreach (var omittedAction in this.definedActions.Where(action => !this.registry.ContainsKey(action))) {
          Logger.DebugFormat("Missing action handler for {0}", omittedAction.Name);
        }

        return new ActionSet(this.registry);
      }
    }
  }
}
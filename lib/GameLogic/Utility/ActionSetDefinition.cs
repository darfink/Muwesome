using System;
using System.Collections.Generic;

namespace Muwesome.GameLogic.Utility {
  internal sealed class ActionSetDefinition {
    private readonly ISet<Type> definedActions = new HashSet<Type>();

    /// <summary>Initializes a new instance of the <see cref="ActionSetDefinition"/> class.</summary>
    public ActionSetDefinition(params Type[] definedActions) {
      foreach (var actionType in definedActions) {
        if (!typeof(Delegate).IsAssignableFrom(actionType)) {
          throw new ArgumentException($"Any defined action must be a {nameof(Delegate)}", nameof(definedActions));
        }

        if (!this.definedActions.Add(actionType)) {
          throw new ArgumentException($"{actionType.Name} was defined more than once", nameof(definedActions));
        }
      }
    }

    /// <summary>Creates a new action set from a collection of actions.</summary>
    public ActionSet WithActions(IEnumerable<Delegate> actions) {
      var builder = new ActionSet.Builder(this.definedActions);

      foreach (var action in actions) {
        builder.RegisterAction(action);
      }

      return builder.Build();
    }
  }
}
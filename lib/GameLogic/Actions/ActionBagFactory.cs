using System;
using System.Collections.Generic;
using System.Linq;
using log4net;

namespace Muwesome.GameLogic.Actions {
  /// <summary>Builds an action bag by registering all its actions.</summary>
  internal delegate void ActionBagBuilder<T>(T context, Action<Delegate> registerAction);

  internal sealed class ActionBagFactory {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(ActionBagFactory));

    /// <summary>Initializes a new instance of the <see cref="ActionBagFactory"/> class.</summary>
    public ActionBagFactory(params Type[] definedActions) {
      foreach (var actionType in definedActions) {
        if (!typeof(Delegate).IsAssignableFrom(actionType)) {
          throw new ArgumentException($"Any defined action must be a {nameof(Delegate)}", nameof(definedActions));
        }

        if (!this.DefinedActions.Add(actionType)) {
          throw new ArgumentException($"{actionType.Name} was defined more than once", nameof(definedActions));
        }
      }
    }

    /// <summary>Gets the defined actions for this factory.</summary>
    public ISet<Type> DefinedActions { get; } = new HashSet<Type>();

    /// <summary>Creates a new action bag.</summary>
    public ActionBag Create<TContext>(TContext context, ActionBagBuilder<TContext> builder) {
      var registeredActions = new Dictionary<Type, Delegate>();
      builder(context, action => this.RegisterAction(registeredActions, action));

      foreach (var omittedAction in this.DefinedActions.Where(action => !registeredActions.ContainsKey(action))) {
        Logger.Debug($"Missing action handler for {omittedAction.Name}");
      }

      return new ActionBag(registeredActions);
    }

    /// <summary>Adds an action an action registry.</summary>
    private void RegisterAction(Dictionary<Type, Delegate> registry, Delegate action) {
      var actionType = action.GetType();

      if (!this.DefinedActions.Contains(actionType)) {
        throw new ArgumentException($"Undefined action type {actionType.Name}");
      }

      if (registry.ContainsKey(actionType)) {
        throw new ArgumentException($"Duplicated action type {actionType.Name}");
      }

      registry[actionType] = action;
    }
  }
}
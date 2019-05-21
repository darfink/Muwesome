using System;
using System.Collections.Generic;
using System.Linq;
using Muwesome.GameLogic;
using Muwesome.GameLogic.Actions.Players;
using Muwesome.GameLogic.Actions.Players.Handlers;
using Muwesome.MethodDelegate.Extensions;

namespace Muwesome.GameLogic.Actions {
  internal class PlayerActionFactory {
    private readonly List<object> actionHandlers = new List<object>();
    private readonly ActionBagFactory actionBagFactory;

    /// <summary>Initializes a new instance of the <see cref="PlayerActionFactory"/> class.</summary>
    public PlayerActionFactory(ILoginService loginService) {
      this.actionBagFactory = new ActionBagFactory(this.GetDefinedPlayerActions());

      // TODO: Discover and add action handlers via reflection
      this.actionHandlers.Add(new LoginActionHandler(loginService));
      this.actionHandlers.Add(new CharacterActionHandler());
    }

    /// <summary>Creates a new player action collection.</summary>
    public ActionBag Create(Player player, PlayerActionBuilder actionBuilder) =>
      this.actionBagFactory.Create(player, new ActionBagBuilder<Player>(this.RegisterActions + actionBuilder));

    /// <summary>Registers all assembly local actions.</summary>
    private void RegisterActions(Player player, Action<Delegate> registerAction) {
      foreach (var action in this.actionHandlers.SelectMany(a => a.GetMethodDelegates(_ => player))) {
        registerAction(action);
      }
    }

    /// <summary>Gets all defined player action types.</summary>
    private Type[] GetDefinedPlayerActions() {
      var actionRoot = typeof(Actions.Players.LoginAction);
      return actionRoot.Assembly
        .GetTypes()
        .Where(t => t.Namespace == actionRoot.Namespace && typeof(Delegate).IsAssignableFrom(t))
        .ToArray();
    }
  }
}
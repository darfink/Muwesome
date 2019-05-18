using System;
using System.Linq;
using Muwesome.GameLogic;
using Muwesome.GameLogic.Actions.Players;
using Muwesome.GameLogic.Actions.Players.Handlers;

namespace Muwesome.GameLogic.Actions {
  internal class PlayerActionFactory {
    private readonly ActionBagFactory actionBagFactory;
    private readonly LoginActionHandler loginActionHandler;
    private readonly CharacterActionHandler characterActionHandler;

    /// <summary>Initializes a new instance of the <see cref="PlayerActionFactory"/> class.</summary>
    public PlayerActionFactory(ILoginService loginService) {
      this.actionBagFactory = new ActionBagFactory(this.GetDefinedPlayerActions());
      this.loginActionHandler = new LoginActionHandler(loginService);
      this.characterActionHandler = new CharacterActionHandler();
    }

    /// <summary>Creates a new player action collection.</summary>
    public ActionBag Create(Player player, PlayerActionBuilder actionBuilder) =>
      this.actionBagFactory.Create(player, new ActionBagBuilder<Player>(this.RegisterActions + actionBuilder));

    /// <summary>Registers all assembly local actions.</summary>
    private void RegisterActions(Player player, Action<Delegate> registerAction) {
      // TODO: Discover and add action handlers via reflection
      registerAction((this.loginActionHandler as IActionProvider<Player, LoginAction>).CreateAction(player));
      registerAction((this.loginActionHandler as IActionProvider<Player, LogoutAction>).CreateAction(player));
      registerAction((this.characterActionHandler as IActionProvider<Player, RequestCharactersAction>).CreateAction(player));
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
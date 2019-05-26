using System;
using System.Collections.Generic;
using System.Linq;
using Muwesome.DomainModel.Configuration;
using Muwesome.GameLogic.EventHandlers;
using Muwesome.GameLogic.Interface;
using Muwesome.GameLogic.Interface.Actions;
using Muwesome.GameLogic.Interface.Events;
using Muwesome.GameLogic.Utility;
using Muwesome.MethodDelegate.Extensions;
using Muwesome.Persistence;

namespace Muwesome.GameLogic {
  /// <summary>A game's context.</summary>
  public class GameContext {
    private readonly IPersistenceContextProvider persistenceContextProvider;
    private readonly ActionSetDefinition playerActionSetDefinition;
    private readonly GameConfiguration gameConfig;

    /// <summary>Initializes a new instance of the <see cref="GameContext"/> class.</summary>
    public GameContext(IPersistenceContextProvider persistenceContextProvider, ILoginService loginService) {
      this.persistenceContextProvider = persistenceContextProvider;
      this.playerActionSetDefinition = new ActionSetDefinition(PlayerActions.Types);
      this.PlayerEventDispatchers = this.CreatePlayerEventDispatchers(loginService);

      using (var context = this.persistenceContextProvider.CreateContext()) {
        // TODO: The game configuration should be injected
        this.gameConfig = context.GetAll<GameConfiguration>().First();
      }
    }

    /// <summary>Gets the game's players.</summary>
    public IList<Player> Players { get; } = new List<Player>();

    /// <summary>Gets the player event dispatchers.</summary>
    public IActionSet PlayerEventDispatchers { get; private set; }

    /// <summary>Adds a player to the game.</summary>
    public Player AddPlayer(IEnumerable<Delegate> playerActions) {
      var player = new Player(
        this.persistenceContextProvider.CreateAccountContext(),
        this.playerActionSetDefinition.WithActions(playerActions));

      this.Players.Add(player);
      player.Disposed += (_, ev) => this.Players.Remove(player);
      player.Action<ShowLoginWindow>()?.Invoke();
      return player;
    }

    // TODO: Dynamically discover and add event handlers
    private ActionSet CreatePlayerEventDispatchers(ILoginService loginService) =>
      new ActionSetDefinition(PlayerEvents.Types).WithActions(new object[] {
        new LoginHandler(loginService),
        new CharacterHandler(),
      }.SelectMany(handler => handler.GetMethodDelegates()));
  }
}
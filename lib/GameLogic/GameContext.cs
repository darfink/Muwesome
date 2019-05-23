using System;
using System.Collections.Generic;
using System.Linq;
using Muwesome.DomainModel.Configuration;
using Muwesome.GameLogic.Actions;
using Muwesome.GameLogic.PlayerActions;
using Muwesome.Persistence;

namespace Muwesome.GameLogic {
  /// <summary>Builds a player's actions by registration.</summary>
  public delegate void PlayerActionBuilder(Player player, Action<Delegate> registerAction);

  /// <summary>A game's context.</summary>
  public class GameContext {
    private readonly IPersistenceContextProvider persistenceContextProvider;
    private readonly PlayerActionFactory playerActionFactory;
    private readonly GameConfiguration gameConfig;

    /// <summary>Initializes a new instance of the <see cref="GameContext"/> class.</summary>
    public GameContext(IPersistenceContextProvider persistenceContextProvider, ILoginService loginService) {
      this.playerActionFactory = new PlayerActionFactory(loginService);
      this.persistenceContextProvider = persistenceContextProvider;

      using (var context = this.persistenceContextProvider.CreateContext()) {
        // TODO: The game configuration should be injected
        this.gameConfig = context.GetAll<GameConfiguration>().First();
      }
    }

    /// <summary>Gets the game's players.</summary>
    public IList<Player> Players { get; } = new List<Player>();

    /// <summary>Adds a player to the game.</summary>
    public Player AddPlayer(PlayerActionBuilder actionBuilder) {
      var player = new Player(this.persistenceContextProvider.CreateAccountContext());
      this.Players.Add(player);
      player.Disposed += (_, ev) => this.Players.Remove(player);
      player.Actions = this.playerActionFactory.Create(player, actionBuilder);
      player.Action<ShowLoginWindowAction>()?.Invoke();
      return player;
    }
  }
}
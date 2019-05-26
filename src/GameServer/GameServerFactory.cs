using Muwesome.Common;
using Muwesome.GameLogic;
using Muwesome.GameServer.Protocol;
using Muwesome.Network.Tcp.Filters;
using Muwesome.Persistence;

namespace Muwesome.GameServer {
  /// <summary>A factory for game servers.</summary>
  public static class GameServerFactory {
    /// <summary>Initializes a new instance of the <see cref="GameServer" /> class with default implementations.</summary>
    public static GameServer Create(
        Configuration config,
        IPersistenceContextProvider persistenceContextProvider,
        IGameServerRegistrar gameServerRegistrar,
        IAccountLoginService accountLoginService) {
      var gameContext = new GameContext(persistenceContextProvider, new LoginServiceAdapter(accountLoginService));

      var clientController = new ClientController();
      var clientProtocolResolver = new ClientProtocolResolver(config, gameContext);

      var clientListener = new GameServerTcpListener(config, clientController, clientProtocolResolver, gameServerRegistrar);
      clientListener.AddFilter(new MaxConnectionsFilter(config.MaxConnections));
      clientListener.AddFilter(new MaxConnectionsPerIpFilter(config.MaxConnectionsPerIp));

      var gameServer = new GameServer(config, clientController, gameContext);
      gameServer.AddListener(clientListener);
      return gameServer;
    }
  }
}
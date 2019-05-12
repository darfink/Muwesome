using Muwesome.Common;
using Muwesome.GameServer.Protocol;
using Muwesome.Network.Tcp.Filters;
using Muwesome.Persistence;

namespace Muwesome.GameServer {
  /// <summary>A factory for game servers.</summary>
  public static class GameServerFactory {
    /// <summary>Initializes a new instance of the <see cref="GameServer" /> class with default implementations.</summary>
    public static GameServer Create(Configuration config, IGameServerRegistrar gameServerRegistrar) {
      var clientController = new ClientController();
      var clientProtocolResolver = new ClientProtocolResolver(config, clientController);

      var clientListener = new GameServerTcpListener(config, clientController, clientProtocolResolver, gameServerRegistrar);
      clientListener.AddFilter(new MaxConnectionsFilter(config.MaxConnections));
      clientListener.AddFilter(new MaxConnectionsPerIpFilter(config.MaxConnectionsPerIp));

      var gameServer = new GameServer(config, clientController);
      gameServer.AddListener(clientListener);
      return gameServer;
    }
  }
}
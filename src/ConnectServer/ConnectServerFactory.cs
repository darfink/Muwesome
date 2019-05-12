using Muwesome.Network.Tcp.Filters;

namespace Muwesome.ConnectServer {
  public static class ConnectServerFactory {
    /// <summary>Initializes a new instance of the <see cref="ConnectServer" /> class with default implementations.</summary>
    public static ConnectServer Create(Configuration config) {
      var gameServerController = new GameServerController();
      var clientController = new ClientController();
      var clientProtocol = new ClientProtocolHandler(gameServerController, clientController) {
        DisconnectOnUnknownPacket = config.DisconnectOnUnknownPacket,
      };

      var clientListener = new ConnectServerTcpListener(config, clientProtocol);
      clientListener.AddFilter(new MaxConnectionsFilter(config.MaxConnections));
      clientListener.AddFilter(new MaxConnectionsPerIpFilter(config.MaxConnectionsPerIp));

      var connectServer = new ConnectServer(config, gameServerController, clientController);
      connectServer.AddListener(clientListener);
      return connectServer;
    }
  }
}
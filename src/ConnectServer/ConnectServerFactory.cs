using Muwesome.Network.Tcp;

namespace Muwesome.ConnectServer {
  public static class ConnectServerFactory {
    /// <summary>Initializes a new instance of the <see cref="ConnectServer" /> class with default implementations.</summary>
    public static ConnectServer Create(Configuration config) {
      var gameServerController = new GameServerController();
      var clientListener = new DefaultClientTcpListener(config.ClientListenerEndPoint, config.MaxPacketSize);
      var clientController = new ClientController(config.MaxIdleTime);
      var clientProtocol = new ClientProtocolHandler(gameServerController, clientController) {
        DisconnectOnUnknownPacket = config.DisconnectOnUnknownPacket,
      };

      return new ConnectServer(
        config,
        gameServerController,
        clientController,
        clientListener,
        clientProtocol);
    }
  }
}
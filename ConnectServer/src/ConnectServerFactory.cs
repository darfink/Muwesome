using Muwesome.ConnectServer.Rpc;

namespace Muwesome.ConnectServer {
  public static class ConnectServerFactory {
    /// <summary>Initializes a new instance of the <see cref="ConnectServer" /> class with default implementations.</summary>
    public static ConnectServer Create(Configuration config) {
      var gameServerController = new GameServerController();
      var rpcServiceController = new RpcServiceController(config, gameServerController);

      var clientController = new ClientController(config);
      var clientListener = new ClientTcpListener(config);
      var clientProtocol = new ClientProtocolHandler(config, gameServerController, clientController);

      return new ConnectServer(
        config,
        gameServerController,
        clientController,
        clientListener,
        clientProtocol,
        rpcServiceController);
    }
  }
}
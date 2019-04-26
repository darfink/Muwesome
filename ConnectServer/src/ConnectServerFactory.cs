using Muwesome.ConnectServer.Rpc;

namespace Muwesome.ConnectServer {
  public static class ConnectServerFactory {
    /// <summary>Creates a new <see cref="ConnectServer" /> with default implementations.</summary>
    public static ConnectServer Create(Configuration config) {
      var gameServerController = new GameServerController();
      var rpcServiceController = new RpcServiceController(config, gameServerController);

      var clientController = new ClientController(config);
      var clientListener = new ClientTcpListener(config);
      var clientProtocol = new ClientProtocolHandler(config, clientController);

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
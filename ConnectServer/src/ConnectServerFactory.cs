namespace Muwesome.ConnectServer {
  public static class ConnectServerFactory {
    /// <summary>Creates a new <see cref="ConnectServer" /> with default implementations.</summary>
    public static ConnectServer Create(Configuration config) {
      var clientController = new ClientController(config);
      var clientListener = new ClientTcpListener(config);
      var clientProtocol = new ClientProtocolHandler(config, clientController);

      return new ConnectServer(config, clientController, clientListener, clientProtocol);
    }
  }
}
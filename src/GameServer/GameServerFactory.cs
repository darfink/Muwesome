using Muwesome.GameServer.Protocol;
using Muwesome.Network.Tcp;
using Muwesome.Packet.IO.SimpleModulus;
using Muwesome.Packet.IO.Xor;

namespace Muwesome.GameServer {
  /// <summary>A factory for game servers.</summary>
  public static class GameServerFactory {
    /// <summary>Initializes a new instance of the <see cref="GameServer" /> class with default implementations.</summary>
    public static GameServer Create(Configuration config) {
      var clientController = new ClientController(config.MaxIdleTime);
      var clientListener = new DefaultClientTcpListener(config.ClientListenerEndPoint, config.MaxPacketSize) {
        Decryption = reader => new XorPipelineDecryptor(new SimpleModulusPipelineDecryptor(reader).Reader),
        Encryption = null,
      };

      var clientProtocolResolver = new ClientProtocolResolver(config, clientController);
      var gameServerRegisterer = new GameServerRegisterer(config, clientController, clientListener);

      return new GameServer(
        config,
        gameServerRegisterer,
        clientController,
        clientListener,
        clientProtocolResolver);
    }
  }
}
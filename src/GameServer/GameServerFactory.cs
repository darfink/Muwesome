using Muwesome.Common;
using Muwesome.GameServer.Protocol;
using Muwesome.Network.Tcp;
using Muwesome.Packet.IO.SimpleModulus;
using Muwesome.Packet.IO.Xor;
using Muwesome.Persistence;

namespace Muwesome.GameServer {
  /// <summary>A factory for game servers.</summary>
  public static class GameServerFactory {
    /// <summary>Initializes a new instance of the <see cref="GameServer" /> class with default implementations.</summary>
    public static GameServer Create(Configuration config, IGameServerRegistrar gameServerRegistrar) {
      var clientController = new ClientController(config.MaxIdleTime);
      var clientProtocolResolver = new ClientProtocolResolver(config, clientController);
      var clientListener = new DefaultClientTcpListener(config.ClientListenerEndPoint, config.MaxPacketSize) {
        Decryption = reader => new XorPipelineDecryptor(new SimpleModulusPipelineDecryptor(reader).Reader),
        Encryption = null,
      };

      return new GameServer(
        config,
        clientController,
        clientListener,
        clientProtocolResolver,
        gameServerRegistrar);
    }
  }
}
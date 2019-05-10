using Muwesome.GameServer.Protocol;
using Muwesome.Network.Tcp;
using Muwesome.Packet.IO.SimpleModulus;
using Muwesome.Packet.IO.Xor;
using Muwesome.Persistence;
using Muwesome.Persistence.EntityFramework;

namespace Muwesome.GameServer {
  /// <summary>A factory for game servers.</summary>
  public static class GameServerFactory {
    /// <summary>Initializes a new instance of the <see cref="GameServer" /> class with default implementations.</summary>
    public static GameServer Create(Configuration config, IPersistenceContextProvider persistenceContextProvider = null) {
      var contextProvider = persistenceContextProvider ?? new PersistenceContextProvider(config.PersistenceConfiguration);

      var clientController = new ClientController(config.MaxIdleTime);
      var clientListener = new DefaultClientTcpListener(config.ClientListenerEndPoint, config.MaxPacketSize) {
        Decryption = reader => new XorPipelineDecryptor(new SimpleModulusPipelineDecryptor(reader).Reader),
        Encryption = null,
      };

      var clientProtocolResolver = new ClientProtocolResolver(config, clientController);
      var gameServerRegisterer = new GameServerRegisterer(config, clientController, clientListener);
      var accountAuthenticator = new AccountAuthenticator(config, contextProvider);

      return new GameServer(
        config,
        gameServerRegisterer,
        clientController,
        clientListener,
        clientProtocolResolver,
        accountAuthenticator);
    }
  }
}
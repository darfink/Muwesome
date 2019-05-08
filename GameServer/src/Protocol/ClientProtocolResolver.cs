using System.Collections.Generic;
using System.Threading;
using log4net;
using Muwesome.GameServer.Protocol.Dispatchers;
using Muwesome.GameServer.Protocol.Handlers;
using Muwesome.Protocol;
using Muwesome.Protocol.Game;

namespace Muwesome.GameServer.Protocol {
  /// <summary>A client protocol resolver.</summary>
  internal class ClientProtocolResolver : IClientProtocolResolver {
    // private static readonly ILog Logger = LogManager.GetLogger(typeof(LoginRequestHandler));
    private readonly object syncLock = new object();
    private readonly Dictionary<ClientVersion, ClientPacketHandler> packetHandlers = new Dictionary<ClientVersion, ClientPacketHandler>();
    private readonly Dictionary<ClientVersion, ClientPacketDispatcher> packetDispatchers = new Dictionary<ClientVersion, ClientPacketDispatcher>();
    private readonly IClientController clientController;
    private readonly Configuration config;

    /// <summary>Initializes a new instance of the <see cref="ClientProtocolResolver"/> class.</summary>
    public ClientProtocolResolver(Configuration config, IClientController clientController) {
      this.clientController = clientController;
      this.config = config;
    }

    /// <inheritdoc />
    public ClientProtocol Resolve(ClientVersion clientVersion) {
      lock (this.syncLock) {
        if (!this.packetHandlers.TryGetValue(clientVersion, out ClientPacketHandler packetHandler)) {
          this.packetHandlers[clientVersion] = packetHandler = this.CreatePacketHandler(clientVersion);
        }

        if (!this.packetDispatchers.TryGetValue(clientVersion, out ClientPacketDispatcher packetDispatcher)) {
          this.packetDispatchers[clientVersion] = packetDispatcher = this.CreatePacketDispatcher(clientVersion);
        }

        return new ClientProtocol(packetHandler, packetDispatcher);
      }
    }

    /// <summary>Creates a packet handler for a specific client version.</summary>
    private ClientPacketHandler CreatePacketHandler(ClientVersion version) {
      var clientPacketHandler = new ClientPacketHandler() {
        DisconnectOnUnknownPacket = this.config.DisconnectOnUnknownPacket,
      };

      // TODO: Implement dynamic client version range and do this dynamically
      clientPacketHandler.Register<LoginRequest>(new LoginRequestHandler());

      return clientPacketHandler;
    }

    /// <summary>Creates a packet handler for a specific client version.</summary>
    private ClientPacketDispatcher CreatePacketDispatcher(ClientVersion version) {
      var clientPacketDispatcher = new ClientPacketDispatcher();

      // TODO: Implement dynamic client version range and do this dynamically
      clientPacketDispatcher.Register(new LoginResultDispatcher(this.clientController));
      clientPacketDispatcher.Register(new JoinResultDispatcher(this.clientController));

      return clientPacketDispatcher;
    }
  }
}
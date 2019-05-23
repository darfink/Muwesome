using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using Muwesome.GameServer.Protocol.Dispatchers;
using Muwesome.GameServer.Protocol.Handlers;
using Muwesome.Protocol;
using Muwesome.Protocol.Game;
using Muwesome.Protocol.Game.Client;

namespace Muwesome.GameServer.Protocol {
  /// <summary>A client protocol resolver.</summary>
  // TODO: Implement dynamic client version range and do this dynamically
  internal class ClientProtocolResolver : IClientProtocolResolver {
    private readonly object syncLock = new object();
    private readonly Dictionary<ClientVersion, ClientProtocol> protocols = new Dictionary<ClientVersion, ClientProtocol>();
    private readonly Configuration config;

    /// <summary>Initializes a new instance of the <see cref="ClientProtocolResolver"/> class.</summary>
    public ClientProtocolResolver(Configuration config) => this.config = config;

    /// <inheritdoc />
    public ClientProtocol Resolve(ClientVersion clientVersion) {
      lock (this.syncLock) {
        if (!this.protocols.TryGetValue(clientVersion, out ClientProtocol protocol)) {
          this.protocols[clientVersion] = protocol = new ClientProtocol(
            clientVersion,
            this.CreatePacketHandler(clientVersion),
            this.CreatePacketDispatcher(clientVersion));
        }

        return protocol;
      }
    }

    /// <summary>Creates a packet handler for a specific client version.</summary>
    private ClientPacketHandler CreatePacketHandler(ClientVersion version) {
      var handlers = Assembly
        .GetAssembly(typeof(ClientProtocolResolver))
        .GetTypes()
        .Where(type => !type.IsAbstract && typeof(PacketHandler).IsAssignableFrom(type))
        .Select(Activator.CreateInstance)
        .Cast<PacketHandler>()
        .ToList();

      foreach (var serialValidator in handlers.OfType<IClientSerialValidator>()) {
        serialValidator.ValidateClientSerial = this.config.ValidateClientSerial;
      }

      return new ClientPacketHandler(handlers) {
        DisconnectOnUnknownPacket = this.config.DisconnectOnUnknownPacket,
      };
    }

    /// <summary>Creates a packet dispatcher for a specific client version.</summary>
    private ClientPacketDispatcher CreatePacketDispatcher(ClientVersion version) {
      var dispatchers = Assembly
        .GetAssembly(typeof(ClientProtocolResolver))
        .GetTypes()
        .Where(type => !type.IsAbstract && typeof(PacketDispatcher).IsAssignableFrom(type))
        .Select(Activator.CreateInstance)
        .Cast<PacketDispatcher>();

      return new ClientPacketDispatcher(dispatchers);
    }
  }
}
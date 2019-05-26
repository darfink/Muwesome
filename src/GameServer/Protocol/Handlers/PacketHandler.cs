using System;
using Muwesome.GameLogic;
using Muwesome.Packet;
using Muwesome.Packet.Utility;
using Muwesome.Protocol;

namespace Muwesome.GameServer.Protocol.Handlers {
  /// <summary>A packet handler for clients.</summary>
  internal abstract class PacketHandler : IPacketHandler<Client> {
    /// <summary>Gets a packet handler's identifier.</summary>
    public PacketIdentifier Identifier => ProtocolPacketAttribute.Get(this.GetType()).Packet;

    /// <summary>Gets or sets the associated game context.</summary>
    public GameContext GameContext { get; set; }

    /// <inheritdoc />
    public abstract bool HandlePacket(Client client, Span<byte> packet);

    /// <summary>Gets a player event dispatcher.</summary>
    protected T PlayerEventDispatcher<T>()
        where T : Delegate => this.GameContext.PlayerEventDispatchers.Get<T>();
  }
}
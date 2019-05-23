using System;
using Muwesome.Packet;
using Muwesome.Packet.Utility;
using Muwesome.Protocol;

namespace Muwesome.GameServer.Protocol.Handlers {
  /// <summary>A packet handler for clients.</summary>
  internal abstract class PacketHandler : IPacketHandler<Client> {
    /// <summary>Gets a packet handler's identifier.</summary>
    public PacketIdentifier Identifier => ProtocolPacketAttribute.Get(this.GetType()).Packet;

    /// <inheritdoc />
    public abstract bool HandlePacket(Client client, Span<byte> packet);
  }
}
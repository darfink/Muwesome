using System;
using Muwesome.Packet;
using Muwesome.Packet.Utility;
using Muwesome.Protocol;

namespace Muwesome.GameServer.Protocol {
  /// <summary>A packet handler for clients.</summary>
  internal abstract class PacketHandler<TPacket> : IPacketHandler<Client>
      where TPacket : IPacket {
    /// <summary>Gets the type of packet this instance handles.</summary>
    public PacketIdentifier Identifier => PacketIdentifierFor<TPacket>.Identifier;

    /// <inheritdoc />
    public abstract bool HandlePacket(Client client, Span<byte> packet);
  }
}
using System;
using Muwesome.Packet;
using Muwesome.Packet.Utility;
using Muwesome.Protocol;

namespace Muwesome.GameServer.Protocol.Handlers {
  /// <summary>A packet handler for clients.</summary>
  internal abstract class PacketHandler<TPacket> : IPacketHandler<Client>
      where TPacket : IPacket {
    /// <summary>Gets the type of packet this instance handles.</summary>
    public PacketIdentifier Identifier => PacketIdentifier.Get<TPacket>();

    /// <inheritdoc />
    public abstract bool HandlePacket(Client client, Span<byte> packet);
  }
}
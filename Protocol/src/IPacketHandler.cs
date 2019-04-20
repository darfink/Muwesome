using System;
using Muwesome.Packet;

namespace Muwesome.Protocol {
  public interface IPacketHandler<in T> {
    /// <summary>Handles an incoming packet.</summary>
    /// <returns>Boolean indicating whether the packet was handled or not.</returns>
    bool HandlePacket(T sender, Span<byte> packet);
  }
}
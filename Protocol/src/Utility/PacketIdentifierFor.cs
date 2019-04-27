using System;
using Muwesome.Packet;

namespace Muwesome.Protocol.Utility {
  public static class PacketIdentifierFor<T> where T : IPacket {
    private static readonly PacketAttribute _attribute;

    /// <summary>Caches the packet's identifier.</summary>
    static PacketIdentifierFor() =>
      _attribute = (PacketAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(PacketAttribute), false);

    /// <summary>Gets the packet's identifier.</summary>
    public static PacketIdentifier Identifier => _attribute.Identifier;
  }
}
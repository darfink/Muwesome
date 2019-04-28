using System;
using Muwesome.Packet;

namespace Muwesome.Packet.Utility {
  [AttributeUsage(AttributeTargets.Struct)]
  public class PacketAttribute : Attribute {
    /// <summary>Initializes a new instance of the <see cref="PacketAttribute"/> class.</summary>
    public PacketAttribute(byte type, byte code, params byte[] subcodes) {
      // TODO: Packet name?
      this.Identifier = new PacketIdentifier(string.Empty, type, code, subcodes);
    }

    /// <summary>Gets the attribute's identifier.</summary>
    public PacketIdentifier Identifier { get; }
  }
}
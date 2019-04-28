using System;
using Muwesome.Packet;

namespace Muwesome.Packet.Utility {
  [AttributeUsage(AttributeTargets.Struct)]
  public class PacketAttribute : Attribute {
    /// <summary>Creates a new <see cref="PacketAttribute" />.</summary>
    public PacketAttribute(byte type, byte code, params byte[] subcodes) {
      // TODO: Packet name?
      Identifier = new PacketIdentifier("", type, code, subcodes);
    }

    /// <summary>Gets the attribute's identifier.</summary>
    public PacketIdentifier Identifier { get; set; }
  }
}
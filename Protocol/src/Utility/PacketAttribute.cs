using System;
using Muwesome.Packet;

namespace Muwesome.Protocol.Utility {
  [AttributeUsage(AttributeTargets.Struct)]
  internal class PacketAttribute : Attribute {
    public PacketAttribute(byte type, byte code, params byte[] subcodes) {
      // TODO: Packet name?
      Identifier = new PacketIdentifier("", type, code, subcodes);
    }

    public PacketIdentifier Identifier { get; set; }
  }
}
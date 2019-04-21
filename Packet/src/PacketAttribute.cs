using System;
using System.Collections.Generic;
using System.Linq;

namespace Muwesome.Packet {
  [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
  public sealed class PacketAttribute : Attribute {
    /// <summary>Constructs a new packet attribute.</summary>
    public PacketAttribute(byte type, byte code, params byte[] subcode) {
      Type = type;
      Code = code;
      Subcode = subcode;
      Identifier = new byte[] { Type, Code, }.Concat(Subcode).ToArray();
    }

    /// <summary>Gets or sets the attribute's size.</summary>
    public int Size { get; set; }

    /// <summary>Gets the attribute's type.</summary>
    internal PacketType Type { get; }

    /// <summary>Gets the attribute's code.</summary>
    internal byte Code { get; }

    /// <summary>Gets the attribute's subcode.</summary>
    internal byte[] Subcode { get; }

    /// <summary>Gets the packet's identifier.</summary>
    internal byte[] Identifier { get; }
  }
}
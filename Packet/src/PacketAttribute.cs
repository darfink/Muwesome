using System;

namespace Muwesome.Packet {
  [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
  public sealed class PacketAttribute : Attribute {
    /// <summary>Constructs a new packet attribute.</summary>
    public PacketAttribute(byte type, byte code, params byte[] subcode) {
      Type = type;
      Code = code;
      Subcode = subcode;
    }

    /// <summary>Gets the attribute's type.</summary>
    public byte Type { get; }

    /// <summary>Gets the attribute's code.</summary>
    public byte Code { get; }

    /// <summary>Gets the attribute's subcode.</summary>
    public byte[] Subcode { get; }
  }
}
using System;
using System.Collections.Generic;

namespace Muwesome.Packet {
  public abstract class PacketDefinition {
    private PacketAttribute _attributes;

    /// <summary>Constructs a new packet instance.</summary>
    public PacketDefinition() {
      if (!Attribute.IsDefined(this.GetType(), typeof(PacketAttribute))) {
        throw new InvalidOperationException($"The {this.GetType().Name} class is missing a {nameof(PacketAttribute)}");
      }
    }

    /// <summary>Gets the packet's type.</summary>
    public byte Type => Info.Type;

    /// <summary>Gets the packet's code.</summary>
    public byte Code => Info.Code;

    /// <summary>Gets the packet's subcode.</summary>
    public byte[] Subcode => Info.Subcode;

    /// <summary>Gets the packet's identifier.</summary>
    public IEnumerable<byte> Identifier {
      get {
        yield return Type;
        yield return Code;
        foreach (var subcode in Subcode) {
          yield return subcode;
        }
      }
    }

    private PacketAttribute Info =>
      _attributes ?? (_attributes = GetAttribute());

    private PacketAttribute GetAttribute() =>
      (PacketAttribute) Attribute.GetCustomAttribute(this.GetType(), typeof(PacketAttribute));
  }
}
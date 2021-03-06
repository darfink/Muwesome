using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Muwesome.Packet.Utility;

namespace Muwesome.Packet {
  public sealed class PacketIdentifier {
    private readonly ArraySegment<byte> identifier;

    /// <summary>Initializes a new instance of the <see cref="PacketIdentifier"/> class.</summary>
    public PacketIdentifier(string name, PacketType type, byte code, params byte[] subcode) {
      this.Name = name;
      this.Type = type;
      this.Code = code;
      this.Subcode = Array.AsReadOnly(subcode);
      this.identifier = new ArraySegment<byte>(new byte[] { this.Type, this.Code }.Concat(this.Subcode).ToArray());
    }

    /// <summary>Gets the packet's name.</summary>
    public string Name { get; }

    /// <summary>Gets the packet's type.</summary>
    public PacketType Type { get; }

    /// <summary>Gets the packet's code.</summary>
    public byte Code { get; }

    /// <summary>Gets the packet's subcode.</summary>
    public IReadOnlyList<byte> Subcode { get; }

    /// <summary>Gets the packet's identifier.</summary>
    public IReadOnlyList<byte> Identifier => this.identifier;

    /// <summary>Gets the packet's payload offset.</summary>
    public int PayloadOffset => this.Type.HeaderLength + this.Subcode.Count + 1;

    /// <summary>Gets a type's packet identifier</summary>
    public static PacketIdentifier Get(Type type) => PacketAttribute.Get(type).Identifier;

    /// <summary>Gets a type argument's packet identifier</summary>
    public static PacketIdentifier Get<T>()
        where T : IPacket => PacketIdentifierFor<T>.Identifier;

    /// <summary>Creates a buffer with the identifier preset.</summary>
    public Span<byte> CreateBuffer(int size) {
      var packet = new byte[size];
      this.CopyTo(packet, size);
      return packet;
    }

    /// <summary>Copies the packet's identifier to a destination.</summary>
    public void CopyTo(Span<byte> packet, int payloadSize) {
      this.Type.WriteHeader(packet, this.PayloadOffset + payloadSize, ((ReadOnlySpan<byte>)this.identifier).Slice(1));
    }

    /// <summary>Throws an exception if the packet header does not match.</summary>
    public void EnsureMatchingHeader(Span<byte> data, int minimumPayloadSize) {
      var packet = new PacketView(data);

      if (!packet.HasIdentifier(this.Identifier)) {
        // TODO: Specialized exception
        throw new ArgumentException(nameof(data));
      }

      if (packet.Length < this.PayloadOffset + minimumPayloadSize) {
        // TODO: Specialized exception
        throw new ArgumentOutOfRangeException(nameof(data));
      }

      if (packet.Length != this.Type.ReadSize(data)) {
        // TODO: Specialized exception
        throw new ArgumentException(nameof(data));
      }
    }

    /// <summary>Cache for a type's packet identifier.</summary>
    internal static class PacketIdentifierFor<T>
        where T : IPacket {
      private static readonly PacketAttribute Attribute;

      /// <summary>Initializes static members of the <see cref="PacketIdentifierFor{T}" /> class.</summary>
      static PacketIdentifierFor() => Attribute = PacketAttribute.Get(typeof(T));

      /// <summary>Gets the packet's identifier.</summary>
      public static PacketIdentifier Identifier => Attribute.Identifier;
    }
  }
}
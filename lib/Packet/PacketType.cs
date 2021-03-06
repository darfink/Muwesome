using System;
using System.Collections.Generic;
using Muwesome.Packet.Utility;

namespace Muwesome.Packet {
  public struct PacketType {
    /// <summary>A set of all valid packet types.</summary>
    public static readonly HashSet<byte> ValidTypes = new HashSet<byte> { 0xC1, 0xC2, 0xC3, 0xC4 };

    /// <summary>The unencrypted one-byte packet type.</summary>
    public static readonly PacketType C1 = new PacketType(0xC1);

    /// <summary>The unencrypted two-byte packet type.</summary>
    public static readonly PacketType C2 = new PacketType(0xC2);

    /// <summary>The encrypted one-byte packet type.</summary>
    public static readonly PacketType C3 = new PacketType(0xC3);

    /// <summary>The encrypted two-byte packet type.</summary>
    public static readonly PacketType C4 = new PacketType(0xC4);

    /// <summary>Initializes a new instance of the <see cref="PacketType"/> struct.</summary>
    private PacketType(byte value) => this.Type = value;

    /// <summary>Gets the packet type.</summary>
    /// <remarks>
    /// There are four defined types; C1, C2, C3 and C4 where the latter two are encrypted.
    /// </remarks>
    public byte Type { get; }

    /// <summary>Gets the packet type's header length.</summary>
    /// <remarks>
    /// The header consists of the packet type (C1-C4) and the size field.
    /// </remarks>
    public int HeaderLength => this.SizeFieldLength + 1;

    /// <summary>Gets the length of the packet type's size field in bytes.</summary>
    /// <remarks>
    /// The size is specified using one byte when the packet type is C1 or C3,
    /// and two bytes (Big-endian) when the packet type is C2 or C4.
    /// </remarks>
    public int SizeFieldLength => this.Type == 0xC1 || this.Type == 0xC3 ? 1 : 2;

    /// <summary>Gets a value indicating whether this is an encyrpted packet type or not.</summary>
    /// <remarks>
    /// Only "SimpleModulus" encryption can be identified using the packet type.
    /// Whether a packet is XOR encrypted or not must be known in advance.
    /// </remarks>
    public bool IsEncrypted => this.Type == 0xC3 || this.Type == 0xC4;

    /// <summary>Gets the corresponding decrypted packet type.</summary>
    public PacketType Decrypted => this.IsEncrypted ? (this.Type == 0xC3 ? C1 : C2) : this;

    /// <summary>Converts a byte to its corresponding packet type.</summary>
    /// <remarks>
    /// This operator throws an exception if the value is out of range. Albeit
    /// not idiomatic, this is to ensure a valid packet type is always used
    /// whilst still being ergonomic.
    /// </remarks>
    public static implicit operator PacketType(byte type) {
      switch (type) {
        case 0xC1: return C1;
        case 0xC2: return C2;
        case 0xC3: return C3;
        case 0xC4: return C4;
        default: throw new ArgumentOutOfRangeException(nameof(type));
      }
    }

    /// <summary>Converts a packet type to its byte value.</summary>
    public static implicit operator byte(PacketType type) => type.Type;

    /// <summary>Reads the size from a packet buffer.</summary>
    public int ReadSize(ReadOnlySpan<byte> data) =>
      this.SizeFieldLength > 1 ? data.ReadUInt16BE(offset: 1) : data.ReadByte(offset: 1);

    /// <summary>Writes the size to a packet buffer.</summary>
    public void WriteSize(Span<byte> data, int size) {
      if (this.SizeFieldLength > 1) {
        data.WriteUInt16BE(checked((ushort)size), offset: 1);
      } else {
        data.WriteByte(checked((byte)size), offset: 1);
      }
    }

    /// <summary>Writes the header to a packet buffer.</summary>
    public void WriteHeader(Span<byte> data, int size, bool validateBufferSize = true) =>
      this.WriteHeader(data, size, ReadOnlySpan<byte>.Empty, validateBufferSize);

    /// <summary>Writes the header to a packet buffer.</summary>
    public void WriteHeader(Span<byte> data, int size, ReadOnlySpan<byte> identifier, bool validateBufferSize = true) {
      data.WriteByte(this.Type);
      this.WriteSize(data, size);
      identifier.CopyTo(data.Slice(this.HeaderLength));

      if (validateBufferSize && data.Length < size) {
        // TODO: Specialized exception
        throw new ArgumentOutOfRangeException(nameof(data));
      }
    }
  }
}
using System;
using System.Diagnostics.Contracts;
using System.Collections.Generic;

namespace Muwesome.Packet {
  public ref struct PacketView {
    /// <summary>Minimum size required for extracting header information.</summary>
    public const int MinimumSize = 3;

    /// <summary>A set of all valid packet types.</summary>
    public static readonly HashSet<byte> ValidTypes = new HashSet<byte> { 0xC1, 0xC2, 0xC3, 0xC4 };

    /// <summary>Constructs a packet view.</summary>
    public PacketView(Span<byte> data) => Data = data;

    /// <summary>Validates the packet's header.</summary>
    public void ValidateHeader() {
      if (Data.Length < MinimumSize) {
        throw new ArgumentException("The provided data is insufficient");
      }

      if (!ValidTypes.Contains(Type)) {
        throw new InvalidPacketTypeException(Data.ToArray());
      }

      if (Length < MinimumSize) {
        throw new InvalidPacketSizeException(Data.ToArray(), Length);
      }
    }

    /// <summary>Gets or sets the packet's length.</summary>
    public int Length {
      get => SizeFieldLength > 1 ? Data.ReadUInt16BE(1) : Data.ReadByte(1);
    }

    /// <summary>Gets the packet's header length.</summary>
    /// <remarks>
    /// The header consists of the packet type (C1-C4) and the size of the entire packet.
    /// </remarks>
    public int HeaderLength => SizeFieldLength + 1;

    /// <summary>Gets the packet's size field length in bytes.</summary>
    /// <remarks>
    /// The size is specified using one byte when the packet type is C1 or C3,
    /// and two bytes (Big-endian) when the packet type is C2 or C4.
    /// </remarks>
    public int SizeFieldLength => Type == 0xC1 || Type == 0xC3 ? 1 : 2;

    /// <summary>Gets whether this view is of a partial or complete packet.</summary>
    public bool IsPartial => Data.Length < Length;

    /// <summary>Gets whether this packet is encrypted or not.</summary>
    /// <remarks>
    /// Only "SimpleModulus" encryption can be identified using the packet type.
    /// Whether a packet is XOR encrypted or not must be known in advance.
    /// </remarks>
    public bool IsEncrypted => Type == 0xC3 || Type == 0xC4;

    /// <summary>Gets the packet's type.</summary>
    /// <remarks>
    /// There are four defined types; C1, C2, C3 or C4 where the latter two are encrypted.
    /// </remarks>
    public byte Type => Data[0];

    /// <summary>Gets the packet's code.</summary>
    /// <remarks>
    /// Besides the packet type, all packets have a required code as well (which
    /// may be followed by one or more subcodes).
    /// </remarks>
    public byte? Code => !IsEncrypted && Data.Length > HeaderLength
      ? (byte?)Data[HeaderLength]
      : null;

    /// <summary>Gets the packet's identifier.</summary>
    /// <remarks>
    /// The packet identifier is the prefix of bytes that uniquely identifies a packet.
    /// An example would be: C1 F1 01 — which is the account login request.
    ///
    /// Since a packet's identifier cannot be known in advance, this property
    /// only returns the packet's data, excluding the bytes used for specifying
    /// the size.
    /// </remarks>
    public EnumerableIdentifier Identifier => new EnumerableIdentifier(this);

    /// <summary>Gets the packet's underlying data.</summary>
    public Span<byte> Data { get; private set; }

    public ref struct EnumerableIdentifier {
      private PacketView _packet;

      /// <summary>Constructs a new <see cref="EnumerableIdentifier" />.</summary>
      public EnumerableIdentifier(PacketView packet) => _packet = packet;

      /// <summary>Gets an iterator over the packet's identifier.</summary>
      public Enumerator GetEnumerator() => new Enumerator(_packet);

      public ref struct Enumerator {
        private PacketView _packet;
        private int _index;

        /// <summary>Constructs a new <see cref="Enumerator" />.</summary>
        public Enumerator(PacketView packet) {
          _packet = packet;
          _index = -1;
        }

        /// <summary>Gets the current identifier byte.</summary>
        public byte Current => _packet.Data[_index];

        /// <summary>Advances the enumerator to the next item.</summary>
        public bool MoveNext() {
          _index += (_index == 0) ? _packet.SizeFieldLength : 1;
          return _index < _packet.Data.Length;
        }
      }
    }
  }
}
using System;
using System.Diagnostics.Contracts;
using System.Collections.Generic;

namespace Muwesome.Packet {
  /// <summary>A view of a packet buffer in memory.</summary>
  public ref struct PacketView {
    /// <summary>Minimum size required for extracting header information.</summary>
    public const int MinimumSize = 3;

    /// <summary>Constructs a view of a packet.</summary>
    public PacketView(ReadOnlySpan<byte> data) => Data = data;

    /// <summary>Validates the packet's header.</summary>
    public void ValidateHeader() {
      if (Data.Length < MinimumSize) {
        throw new ArgumentException("The provided data is insufficient");
      }

      if (!PacketType.ValidTypes.Contains(Data[0])) {
        throw new InvalidPacketTypeException(Data.ToArray());
      }

      if (Length < MinimumSize) {
        throw new InvalidPacketSizeException(Data.ToArray(), Length);
      }
    }

    /// <summary>Checks whether this packet has a specific identifier.</summary>
    /// <remarks>
    /// Since an identifier cannot be known in advance, this only checks as many
    /// bytes as provided by the input. That is, an identifier of only C1 would
    /// match a majority of packets.
    /// </remarks>
    public bool HasIdentifier(IEnumerable<byte> identifier) {
      var enumerator = Identifier.GetEnumerator();
      foreach (byte value in identifier) {
        if (!enumerator.MoveNext() || enumerator.Current != value) {
          return false;
        }
      }
      return true;
    }

    /// <summary>Gets the packet's length.</summary>
    public int Length => Type.ReadSize(Data);

    /// <summary>Gets whether this view is of a partial or complete packet.</summary>
    public bool IsPartial => Data.Length < Length;

    /// <summary>Gets the packet's type.</summary>
    /// <remarks>
    /// There are four defined types; C1, C2, C3 or C4 where the latter two are encrypted.
    /// </remarks>
    public PacketType Type => Data[0];

    /// <summary>Gets the packet's code.</summary>
    /// <remarks>
    /// Besides the packet type, all packets have a required code as well (which
    /// may be followed by one or more subcodes).
    /// </remarks>
    public byte? Code => !Type.IsEncrypted && Data.Length > Type.HeaderLength
      ? (byte?)Data[Type.HeaderLength]
      : null;

    /// <summary>Gets the packet's identifier.</summary>
    /// <remarks>
    /// The packet identifier is the prefix of bytes that uniquely identifies a packet.
    /// An example would be: C1 F1 01 — which is the account login request.
    ///
    /// Since a packet's identifier cannot be known in advance, this property
    /// returns the packet's data (excluding the bytes used for specifying the
    /// size), which includes the payload as well.
    /// </remarks>
    public EnumerableIdentifier Identifier => new EnumerableIdentifier(this);

    /// <summary>Gets the packet's underlying data.</summary>
    public ReadOnlySpan<byte> Data { get; private set; }

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
          _index += (_index == 0) ? _packet.Type.SizeFieldLength + 1 : 1;
          return _index < _packet.Data.Length;
        }
      }
    }
  }
}
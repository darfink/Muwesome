using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Muwesome.Packet {
  /// <summary>A view of a packet buffer in memory.</summary>
  public ref struct PacketView {
    /// <summary>Minimum size required for extracting header information.</summary>
    public const int MinimumSize = 3;

    /// <summary>Initializes a new instance of the <see cref="PacketView"/> struct.</summary>
    public PacketView(ReadOnlySpan<byte> data) => this.Data = data;

    /// <summary>Gets the packet's length.</summary>
    public int Length => this.Type.ReadSize(this.Data);

    /// <summary>Gets a value indicating whether this view is of a partial or complete packet.</summary>
    public bool IsPartial => this.Data.Length < this.Length;

    /// <summary>Gets the packet's type.</summary>
    /// <remarks>
    /// There are four defined types; C1, C2, C3 or C4 where the latter two are encrypted.
    /// </remarks>
    public PacketType Type => this.Data[0];

    /// <summary>Gets the packet's code.</summary>
    /// <remarks>
    /// Besides the packet type, all packets have a required code as well (which
    /// may be followed by one or more subcodes).
    /// </remarks>
    public byte? Code => !this.Type.IsEncrypted && this.Data.Length > this.Type.HeaderLength
      ? (byte?)this.Data[this.Type.HeaderLength]
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

    /// <summary>Validates the packet's header.</summary>
    public void ValidateHeader() {
      if (this.Data.Length < MinimumSize) {
        throw new ArgumentException("The provided data is insufficient");
      }

      if (!PacketType.ValidTypes.Contains(this.Data[0])) {
        throw new InvalidPacketTypeException(this.Data.ToArray());
      }

      if (this.Length < MinimumSize) {
        throw new InvalidPacketSizeException(this.Data.ToArray(), this.Length);
      }
    }

    /// <summary>Checks whether this packet has a specific identifier.</summary>
    /// <remarks>
    /// Since an identifier cannot be known in advance, this only checks as many
    /// bytes as provided by the input. That is, an identifier of only C1/C2
    /// would match more or less all packets.
    /// </remarks>
    public bool HasIdentifier(IEnumerable<byte> identifier) {
      var enumerator = this.Identifier.GetEnumerator();
      foreach (byte value in identifier) {
        if (!enumerator.MoveNext() || enumerator.Current != value) {
          return false;
        }
      }

      return true;
    }

    public ref struct EnumerableIdentifier {
      private PacketView packet;

      /// <summary>Initializes a new instance of the <see cref="EnumerableIdentifier"/> struct.</summary>
      public EnumerableIdentifier(PacketView packet) => this.packet = packet;

      /// <summary>Gets an iterator over the packet's identifier.</summary>
      public Enumerator GetEnumerator() => new Enumerator(this.packet);

      public ref struct Enumerator {
        private PacketView packet;
        private int index;

        /// <summary>Initializes a new instance of the <see cref="Enumerator"/> struct.</summary>
        public Enumerator(PacketView packet) {
          this.packet = packet;
          this.index = -1;
        }

        /// <summary>Gets the current identifier byte.</summary>
        public byte Current => this.packet.Data[this.index];

        /// <summary>Advances the enumerator to the next item.</summary>
        public bool MoveNext() {
          this.index += (this.index == 0) ? this.packet.Type.SizeFieldLength + 1 : 1;
          return this.index < this.packet.Data.Length;
        }
      }
    }
  }
}
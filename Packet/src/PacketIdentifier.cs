using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace Muwesome.Packet {
  public sealed class PacketIdentifier {
    private ArraySegment<byte> _identifier;

    /// <summary>Creates a new packet identifier.</summary>
    public PacketIdentifier(string name, PacketType type, byte code, params byte[] subcode) {
      Name = name;
      Type = type;
      Code = code;
      Subcode = Array.AsReadOnly(subcode);
      _identifier = new ArraySegment<byte>(new byte[] { Type, Code }.Concat(Subcode).ToArray());
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
    public IReadOnlyList<byte> Identifier => _identifier;

    /// <summary>Gets the packet's payload offset.</summary>
    public int PayloadOffset => Type.HeaderLength + Subcode.Count + 1;

    /// <summary>Creates a buffer with the identifier preset.</summary>
    public Span<byte> CreateBuffer(int size) {
      var packet = new byte[size];
      CopyTo(packet, size);
      return packet;
    }

    /// <summary>Copies the packet's identifier into a destination.</summary>
    public void CopyTo(Span<byte> packet, int payloadSize) {
      Type.WriteHeader(packet, ((ReadOnlySpan<byte>)_identifier).Slice(1), PayloadOffset + payloadSize);
    }

    /// <summary>Throws an exception if the packet header does not match.</summary>
    public void EnsureMatchingHeader(Span<byte> data, int minimumPayloadSize) {
      var packet = new PacketView(data);

      if (!packet.HasIdentifier(Identifier)) {
        // TODO: Specialized exception
        throw new ArgumentException(nameof(data));
      }

      if (packet.Length < PayloadOffset + minimumPayloadSize) {
        // TODO: Specialized exception
        throw new ArgumentOutOfRangeException(nameof(data));
      }

      if (packet.Length < Type.ReadSize(data)) {
        // TODO: Specialized exception
        throw new ArgumentException(nameof(data));
      }
    }
  }
}
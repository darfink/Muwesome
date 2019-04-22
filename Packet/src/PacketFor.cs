using System;
using System.Collections.Generic;

namespace Muwesome.Packet {
  /// <summary>Utility class for statically retrieving a packet's properties.</summary>
  public static class PacketFor<T> where T : IPacket {
    private static readonly PacketAttribute _definition;

    /// <summary>Caches the packet's attribute.</summary>
    static PacketFor() =>
      _definition = (PacketAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(PacketAttribute), false);

    /// <summary>Returns the packet's payload — after the identifier is validated.</summary>
    public static Span<byte> AsPayload(Span<byte> packetData) {
      var packet = new PacketView(packetData);

      if (!packet.HasIdentifier(Identifier)) {
        // TODO: Throw specialized exception
        throw new ArgumentException(nameof(packetData));
      }

      int payloadOffset = packet.Type.HeaderLength + Subcode.Length + 1;
      return packetData.Slice(payloadOffset);
    }

    /// <summary>Copies the packet's header into a destination.</summary>
    public static void CopyTo(Span<byte> packet, int? size = null) {
      Type.WriteHeader(packet, Identifier.AsSpan().Slice(1), (size ?? Size).Value);
    }

    /// <summary>Allocates a configured buffer for the packet.</summary>
    public static byte[] Create(int? size = null) {
      var packet = new byte[(size ?? Size).Value];
      CopyTo(packet, size);
      return packet;
    }

    /// <summary>Gets the packet's name.</summary>
    public static string Name => typeof(T).Name;

    /// <summary>Gets the packet's type.</summary>
    public static PacketType Type => _definition.Type;

    /// <summary>Gets the packet's code.</summary>
    public static byte Code => _definition.Code;

    /// <summary>Gets the packet's subcode.</summary>
    public static byte[] Subcode => _definition.Subcode;

    /// <summary>Gets the packet's identifier.</summary>
    public static byte[] Identifier => _definition.Identifier;

    /// <summary>Gets the packet's size.</summary>
    /// <remarks>This is only available for fixed sized packets.</remarks>
    public static int? Size => _definition.Size == 0 ? null : (int?)_definition.Size;
  }
}
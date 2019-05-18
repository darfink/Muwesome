using System;
using System.Runtime.InteropServices;
using Muwesome.Packet;

namespace Muwesome.Packet.Utility {
  public static class PacketHelper {
    public static ref TPacket ParsePacket<TPacket>(Span<byte> data)
        where TPacket : struct, IPacket {
      // TODO: Return ref vs span, most ergonomic?
      var packet = PacketIdentifierFor<TPacket>.Identifier;
      packet.EnsureMatchingHeader(data, Marshal.SizeOf<TPacket>());
      return ref MemoryMarshal.Cast<byte, TPacket>(data.Slice(packet.PayloadOffset))[0];
    }

    public static ref TPacket ParsePacket<TPacket, TInner>(Span<byte> data, out Span<TInner> inner)
        where TPacket : struct, IDynamicPacket<TInner>
        where TInner : struct {
      ref var payload = ref ParsePacket<TPacket>(data);

      int expectedSize = GetPacketSize<TPacket, TInner>(payload.Count);
      if (data.Length < expectedSize) {
        throw new ArgumentOutOfRangeException(nameof(data));
      }

      inner = MemoryMarshal
        .Cast<byte, TInner>(data.Slice(GetPacketMinimumSize<TPacket>()))
        .Slice(0, payload.Count);
      return ref payload;
    }

    public static ref TPacket CreatePacket<TPacket>(out byte[] buffer)
        where TPacket : struct, IPacket {
      buffer = new byte[GetPacketMinimumSize<TPacket>()];
      return ref CreatePacket<TPacket>(buffer.AsSpan());
    }

    public static ref TPacket CreatePacket<TPacket>(Span<byte> data)
        where TPacket : struct, IPacket {
      if (data.Length < GetPacketMinimumSize<TPacket>()) {
        throw new ArgumentOutOfRangeException(nameof(data));
      }

      var packet = PacketIdentifierFor<TPacket>.Identifier;
      packet.CopyTo(data, Marshal.SizeOf<TPacket>());
      ref var output = ref MemoryMarshal.Cast<byte, TPacket>(data.Slice(packet.PayloadOffset))[0];

      if (output is IInitializable initializable) {
        initializable.Initialize();
      }

      return ref output;
    }

    public static ref TPacket CreatePacket<TPacket, TInner>(int count, out byte[] buffer, out Span<TInner> inner)
        where TPacket : struct, IDynamicPacket<TInner>
        where TInner : struct {
      buffer = new byte[GetPacketSize<TPacket, TInner>(count)];
      return ref CreatePacket<TPacket, TInner>(count, buffer.AsSpan(), out inner);
    }

    public static ref TPacket CreatePacket<TPacket, TInner>(int count, Span<byte> data, out Span<TInner> inner)
        where TPacket : struct, IDynamicPacket<TInner>
        where TInner : struct {
      int expectedSize = GetPacketSize<TPacket, TInner>(count);
      if (data.Length < expectedSize) {
        throw new ArgumentOutOfRangeException(nameof(data));
      }

      ref var payload = ref CreatePacket<TPacket>(data);
      payload.Count = count;

      // Replace the current size (which is the minimum size) with the actual size
      PacketIdentifierFor<TPacket>.Identifier.Type.WriteSize(data, expectedSize);

      inner = MemoryMarshal
        .Cast<byte, TInner>(data.Slice(GetPacketMinimumSize<TPacket>()))
        .Slice(0, count);

      if (typeof(IInitializable).IsAssignableFrom(typeof(TInner))) {
        foreach (var innerData in inner) {
          ((IInitializable)innerData).Initialize();
        }
      }

      return ref payload;
    }

    public static int GetPacketMinimumSize<TPacket>()
        where TPacket : struct, IPacket =>
      PacketIdentifierFor<TPacket>.Identifier.PayloadOffset + Marshal.SizeOf<TPacket>();

    public static int GetPacketSize<TPacket>()
        where TPacket : struct, IFixedPacket =>
      GetPacketMinimumSize<TPacket>();

    public static int GetPacketSize<TPacket, TInner>(int count)
        where TPacket : struct, IDynamicPacket<TInner>
        where TInner : struct =>
      GetPacketMinimumSize<TPacket>() + (Marshal.SizeOf<TInner>() * count);
  }
}
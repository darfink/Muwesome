using System;
using Muwesome.Packet;
using Muwesome.Packet.Utility;

namespace Muwesome.Network {
  public struct PacketWriter<T> : IDisposable where T : struct, IFixedPacket {
    private ThreadSafeWriter _writer;

    /// <summary>Creates a new <see cref="PacketWriter" />.</summary>
    internal PacketWriter(ThreadSafeWriter writer) => _writer = writer;

    /// <summary>Gets the packet's payload.</summary>
    public ref T Packet => ref PacketHelper.CreatePacket<T>(_writer.Span);

    /// <inheritdoc />
    public void Dispose() => _writer.Dispose();
  }
}
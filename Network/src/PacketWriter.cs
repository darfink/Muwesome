using System;
using Muwesome.Packet;
using Muwesome.Packet.Utility;

namespace Muwesome.Network {
  public struct PacketWriter<T> : IDisposable
      where T : struct, IFixedPacket {
    private ThreadSafeWriter writer;

    /// <summary>Initializes a new instance of the <see cref="PacketWriter{T}"/> struct.</summary>
    internal PacketWriter(ThreadSafeWriter writer) => this.writer = writer;

    /// <summary>Gets the packet's payload.</summary>
    public ref T Packet => ref PacketHelper.CreatePacket<T>(this.writer.Span);

    /// <inheritdoc />
    public void Dispose() => this.writer.Dispose();
  }
}
using System;
using Muwesome.Packet;
using Muwesome.Packet.Utility;

namespace Muwesome.Network {
  public struct PacketWrite<T> : IDisposable
      where T : struct, IFixedPacket {
    private readonly WriteTransaction write;

    /// <summary>Initializes a new instance of the <see cref="PacketWrite{T}"/> struct.</summary>
    internal PacketWrite(WriteTransaction write) => this.write = write;

    /// <summary>Gets the packet's payload.</summary>
    public ref T Packet => ref PacketHelper.CreatePacket<T>(this.write.Span);

    /// <inheritdoc />
    public void Dispose() => this.write.Dispose();
  }
}
using System;
using Muwesome.Packet;
using Muwesome.Packet.Utility;

namespace Muwesome.Network {
  public struct PacketWriteTransaction<T> : IDisposable
      where T : struct, IFixedPacket {
    private readonly WriteTransaction write;

    /// <summary>Initializes a new instance of the <see cref="PacketWriteTransaction{T}"/> struct.</summary>
    internal PacketWriteTransaction(WriteTransaction write) {
      this.write = write;
      PacketHelper.CreatePacket<T>(this.write.Span);
    }

    /// <summary>Gets the packet's payload.</summary>
    public ref T Packet => ref PacketHelper.ParsePacket<T>(this.write.Span);

    /// <inheritdoc />
    public void Dispose() => this.write.Dispose();
  }
}
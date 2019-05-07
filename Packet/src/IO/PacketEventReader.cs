using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Muwesome.Packet.IO {
  /// <summary>Callback for whenever a new packet is read.</summary>
  public delegate void PacketReadCallback(Span<byte> packet);

  public class PacketEventReader : PacketPipeReaderBase {
    private readonly byte[] packetBuffer;
    private readonly int? maxPacketSize;
    private PacketReadCallback packetCallback;
    private Exception exception;

    /// <summary>Initializes a new instance of the <see cref="PacketEventReader"/> class.</summary>
    public PacketEventReader(PipeReader source, int? maxPacketSize = null) {
      this.packetBuffer = new byte[maxPacketSize ?? 0xFF];
      this.maxPacketSize = maxPacketSize;
      this.Source = source;
    }

    /// <summary>Begins decrypting packets from the source.</summary>
    /// <remarks>
    /// A callback is used due to <see cref="Span"/>'s inability to work with async.
    /// </remarks>
    public async Task BeginRead(PacketReadCallback callback) {
      this.packetCallback = callback;
      await this.ReadSource();

      if (this.exception != null) {
        ExceptionDispatchInfo.Capture(this.exception).Throw();
      }
    }

    /// <inheritdoc/>
    protected override void OnComplete(Exception exception) =>
      this.exception = exception;

    /// <inheritdoc/>
    protected override Task ReadPacket(ReadOnlySequence<byte> packet) {
      if (this.maxPacketSize != null && packet.Length > this.maxPacketSize.Value) {
        var bytes = packet.Slice(0, packet.Length).ToArray();
        throw new MaxPacketSizeExceededException(bytes, (int)packet.Length, this.maxPacketSize.Value);
      }

      Span<byte> packetSpan;
      IMemoryOwner<byte> owner = null;
      if (packet.Length <= this.packetBuffer.Length) {
        packetSpan = this.packetBuffer.AsSpan().Slice(0, (int)packet.Length);
      } else {
        owner = MemoryPool<byte>.Shared.Rent((int)packet.Length);
        packetSpan = owner.Memory.Span.Slice(0, (int)packet.Length);
      }

      packet.CopyTo(packetSpan);
      try {
        this.packetCallback.Invoke(packetSpan);
      } finally {
        owner?.Dispose();
      }

      return Task.CompletedTask;
    }
  }
}
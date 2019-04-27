using System;
using System.Linq;
using System.Buffers;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.IO.Pipelines;

namespace Muwesome.Packet.IO {
  /// <summary>Callback for whenever a new packet is read.</summary>
  public delegate void PacketReadCallback(Span<byte> packet);

  public class PacketEventReader : PacketPipeReaderBase {
    private readonly byte[] _packetBuffer;
    private readonly int? _maxPacketSize;
    private PacketReadCallback _packetCallback;
    private Exception _exception;

    /// <summary>Contructs a new packet reader.</summary>
    public PacketEventReader(PipeReader source, int? maxPacketSize = null) {
      _packetBuffer = new byte[maxPacketSize ?? 0xFF];
      _maxPacketSize = maxPacketSize;
      Source = source;
    }

    /// <summary>Begins decrypting packets from the source.</summary>
    /// <remarks>
    /// A callback is used due to <see cref="Span"/>'s inability to work with async.
    /// </remarks>
    public async Task BeginRead(PacketReadCallback callback) {
      _packetCallback = callback;
      await ReadSource();

      if (_exception != null) {
        ExceptionDispatchInfo.Capture(_exception).Throw();
      }
    }

    /// <inheritdoc/>
    protected override void OnComplete(Exception exception) =>
      _exception = exception;

    /// <inheritdoc/>
    protected override Task ReadPacket(ReadOnlySequence<byte> packet) {
      if (_maxPacketSize != null && packet.Length > _maxPacketSize.Value) {
        var bytes = packet.Slice(0, packet.Length).ToArray();
        throw new MaxPacketSizeExceededException(bytes, (int)packet.Length, _maxPacketSize.Value);
      }

      Span<byte> packetSpan;
      IMemoryOwner<byte> owner = null;
      if (packet.Length <= _packetBuffer.Length) {
        packetSpan = _packetBuffer;
      } else {
        owner = MemoryPool<byte>.Shared.Rent((int)packet.Length);
        packetSpan = owner.Memory.Span.Slice(0, (int)packet.Length);
      }

      packet.CopyTo(packetSpan);
      try {
        _packetCallback.Invoke(packetSpan);
      } finally {
        owner?.Dispose();
      }

      return Task.CompletedTask;
    }
  }
}
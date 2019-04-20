using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading.Tasks;

namespace Muwesome.Packet.IO {
  public class UniformPipelineReader : PacketPipeReaderBase, IPipelineDecryptor {
    private readonly Pipe _pipe = new Pipe();

    /// <summary>Contructs a new uniform pipeline reader.</summary>
    public UniformPipelineReader(PipeReader source) {
      Source = source;
      _ = ReadSource();
    }

    /// <inheritdoc/>
    public PipeReader Reader => _pipe.Reader;

    /// <inheritdoc/>
    protected override void OnComplete(Exception exception) =>
      _pipe.Writer.Complete(exception);

    /// <inheritdoc/>
    protected override async Task ReadPacket(ReadOnlySequence<byte> packet) {
      CopyAndWrite(packet);
      await _pipe.Writer.FlushAsync();
    }

    private void CopyAndWrite(ReadOnlySequence<byte> packet) {
      int packetSize = (int)packet.Length;
      var output = _pipe.Writer.GetSpan(packetSize).Slice(0, packetSize);
      packet.CopyTo(output);
      _pipe.Writer.Advance(packetSize);
    }
  }
}
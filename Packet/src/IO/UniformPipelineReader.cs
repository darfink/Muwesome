using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading.Tasks;

namespace Muwesome.Packet.IO {
  public class UniformPipelineReader : PacketPipeReaderBase, IPipelineDecryptor {
    private readonly Pipe pipe = new Pipe();

    /// <summary>Initializes a new instance of the <see cref="UniformPipelineReader"/> class.</summary>
    public UniformPipelineReader(PipeReader source) {
      this.Source = source;
      _ = this.ReadSource();
    }

    /// <inheritdoc/>
    public PipeReader Reader => this.pipe.Reader;

    /// <inheritdoc/>
    protected override void OnComplete(Exception exception) =>
      this.pipe.Writer.Complete(exception);

    /// <inheritdoc/>
    protected override async Task ReadPacket(ReadOnlySequence<byte> packet) {
      this.CopyAndWrite(packet);
      await this.pipe.Writer.FlushAsync();
    }

    private void CopyAndWrite(ReadOnlySequence<byte> packet) {
      int packetSize = (int)packet.Length;
      var output = this.pipe.Writer.GetSpan(packetSize).Slice(0, packetSize);
      packet.CopyTo(output);
      this.pipe.Writer.Advance(packetSize);
    }
  }
}
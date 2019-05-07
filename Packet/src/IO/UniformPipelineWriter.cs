using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading.Tasks;

namespace Muwesome.Packet.IO {
  public class UniformPipelineWriter : PacketPipeReaderBase, IPipelineEncryptor {
    private readonly Pipe pipe = new Pipe();
    private readonly PipeWriter target;

    /// <summary>Initializes a new instance of the <see cref="UniformPipelineWriter"/> class.</summary>
    public UniformPipelineWriter(PipeWriter target) {
      this.Source = this.pipe.Reader;
      this.target = target;
      _ = this.ReadSource();
    }

    /// <inheritdoc/>
    public virtual PipeWriter Writer => this.pipe.Writer;

    /// <inheritdoc/>
    protected override void OnComplete(Exception exception) =>
      this.target.Complete(exception);

    /// <inheritdoc/>
    protected override async Task ReadPacket(ReadOnlySequence<byte> packet) {
      this.CopyAndWrite(packet);
      await this.target.FlushAsync();
    }

    /// <summary>Writes the data sequence directly to the writer.</summary>
    private void CopyAndWrite(ReadOnlySequence<byte> packet) {
      int packetSize = (int)packet.Length;
      var output = this.target.GetSpan(packetSize).Slice(0, packetSize);
      packet.CopyTo(output);
      this.target.Advance(packetSize);
    }
  }
}
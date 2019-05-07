using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading.Tasks;

namespace Muwesome.Packet.IO.Xor {
  public class XorPipelineDecryptor : XorPipeline, IPipelineDecryptor {
    private readonly Pipe pipe = new Pipe();

    /// <summary>Initializes a new instance of the <see cref="XorPipelineDecryptor"/> class.</summary>
    public XorPipelineDecryptor(PipeReader source)
        : this(source, XorCipher.DefaultKeys) {
    }

    /// <summary>Initializes a new instance of the <see cref="XorPipelineDecryptor"/> class.</summary>
    public XorPipelineDecryptor(PipeReader source, byte[] xorCipher)
        : base(xorCipher) {
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
      this.ApplyCipherAndWrite(this.pipe.Writer, packet, XorOperation.Decrypt);
      await this.pipe.Writer.FlushAsync();
    }
  }
}
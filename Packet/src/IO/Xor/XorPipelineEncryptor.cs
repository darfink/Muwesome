using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading.Tasks;

namespace Muwesome.Packet.IO.Xor {
  public class XorPipelineEncryptor : XorPipeline, IPipelineEncryptor {
    private readonly Pipe pipe = new Pipe();
    private readonly PipeWriter target;

    /// <summary>Initializes a new instance of the <see cref="XorPipelineEncryptor"/> class.</summary>
    public XorPipelineEncryptor(PipeWriter target)
        : this(target, XorCipher.DefaultKeys) {
    }

    /// <summary>Initializes a new instance of the <see cref="XorPipelineEncryptor"/> class.</summary>
    public XorPipelineEncryptor(PipeWriter target, byte[] xorCipher)
        : base(xorCipher) {
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
      this.ApplyCipherAndWrite(this.target, packet, encrypt: true);
      await this.target.FlushAsync();
    }
  }
}
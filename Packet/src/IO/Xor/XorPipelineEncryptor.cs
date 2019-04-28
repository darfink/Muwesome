using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading.Tasks;

namespace Muwesome.Packet.IO.Xor {
  public class XorPipelineEncryptor : XorPipeline, IPipelineEncryptor {
    private readonly Pipe _pipe = new Pipe();
    private readonly PipeWriter _target;

    /// <summary>Contructs a new XOR pipeline encryptor.</summary>
    public XorPipelineEncryptor(PipeWriter target) :
      this(target, XorCipher.DefaultKeys) { }

    /// <summary>Contructs a new XOR pipeline encryptor.</summary>
    public XorPipelineEncryptor(PipeWriter target, byte[] xorCipher) : base(xorCipher) {
      Source = _pipe.Reader;
      _target = target;
      _ = ReadSource();
    }

    /// <inheritdoc/>
    public virtual PipeWriter Writer => _pipe.Writer;

    /// <inheritdoc/>
    protected override void OnComplete(Exception exception) =>
      _target.Complete(exception);

    /// <inheritdoc/>
    protected override async Task ReadPacket(ReadOnlySequence<byte> packet) {
      ApplyCipherAndWrite(_target, packet, encrypt: true);
      await _target.FlushAsync();
    }
  }
}
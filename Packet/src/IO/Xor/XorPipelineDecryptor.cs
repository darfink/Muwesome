using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading.Tasks;

namespace Muwesome.Packet.IO.Xor {
  public class XorPipelineDecryptor : XorPipeline, IPipelineDecryptor {
    private readonly Pipe _pipe = new Pipe();

    /// <summary>Contructs a new XOR pipeline decryptor.</summary>
    public XorPipelineDecryptor(PipeReader source) :
      this(source, XorCipher.DefaultKeys) { }

    /// <summary>Contructs a new XOR pipeline decryptor.</summary>
    public XorPipelineDecryptor(PipeReader source, byte[] xorCipher) : base(xorCipher) {
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
      ApplyCipherAndWrite(_pipe.Writer, packet, encrypt: false);
      await _pipe.Writer.FlushAsync();
    }
  }
}
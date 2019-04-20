using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading.Tasks;

namespace Muwesome.Packet.IO {
  public class UniformPipelineWriter : PacketPipeReaderBase, IPipelineEncryptor {
    private readonly Pipe _pipe = new Pipe();
    private readonly PipeWriter _target;

    /// <summary>Contructs a new uniform pipeline writer.</summary>
    public UniformPipelineWriter(PipeWriter target) {
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
      CopyAndWrite(packet);
      await _target.FlushAsync();
    }

    private void CopyAndWrite(ReadOnlySequence<byte> packet) {
      int packetSize = (int)packet.Length;
      var output = _target.GetSpan(packetSize).Slice(0, packetSize);
      packet.CopyTo(output);
      _target.Advance(packetSize);
    }
  }
}
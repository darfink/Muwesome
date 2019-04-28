using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading.Tasks;

namespace Muwesome.Packet.IO {
  public abstract class PacketPipeReaderBase {
    /// <summary>Sets the source from which the packets can be read from.</summary>
    protected PipeReader Source { private get; set; }

    /// <summary>Reads a packet extracted from the source.</summary>
    protected abstract Task ReadPacket(ReadOnlySequence<byte> packet);

    /// <summary>Called when the source is finished.</summary>
    protected abstract void OnComplete(Exception exception);

    /// <summary>Reads from the source until it is finished.</summary>
    protected async Task ReadSource() {
      Exception exception = null;
      try {
        while (!await this.ReadBuffer()) {
          // Keep processing as long as there is data...
        }
      } catch (Exception ex) {
        exception = ex;
      }

      this.Source.Complete(exception);
      this.OnComplete(exception);
    }

    private async Task<bool> ReadBuffer() {
      ReadResult result = await this.Source.ReadAsync();
      ReadOnlySequence<byte> buffer = result.Buffer;

      while (buffer.Length >= PacketView.MinimumSize) {
        this.ValidatePacket(buffer, out int packetSize);

        if (buffer.Length < packetSize) {
          // Wait for more data...
          break;
        }

        await this.ReadPacket(buffer.Slice(0, packetSize));
        buffer = buffer.Slice(packetSize);
      }

      this.Source.AdvanceTo(buffer.Start);
      return result.IsCanceled || result.IsCompleted;
    }

    private void ValidatePacket(ReadOnlySequence<byte> data, out int packetSize) {
      Span<byte> header = stackalloc byte[PacketView.MinimumSize];
      data.Slice(0, PacketView.MinimumSize).CopyTo(header);

      var packet = new PacketView(header);
      packet.ValidateHeader();
      packetSize = packet.Length;
    }
  }
}
using System;
using System.Buffers;
using System.IO.Pipelines;

namespace Muwesome.Packet.IO.Xor {
  public enum XorOperation {
    Encrypt,
    Decrypt,
  }

  public abstract class XorPipeline : PacketPipeReaderBase {
    private readonly byte[] cipher;

    /// <summary>Initializes a new instance of the <see cref="XorPipeline"/> class.</summary>
    public XorPipeline(byte[] xorCipher) => this.cipher = xorCipher;

    /// <summary>Writes encrypted or decrypted packet data to a pipe.</summary>
    protected void ApplyCipherAndWrite(PipeWriter writer, ReadOnlySequence<byte> packet, XorOperation operation) {
      int packetSize = (int)packet.Length;
      var packetData = writer.GetSpan(packetSize).Slice(0, packetSize);
      packet.CopyTo(packetData);

      if (operation == XorOperation.Encrypt) {
        XorCipher.Encrypt(packetData, this.cipher);
      } else {
        XorCipher.Decrypt(packetData, this.cipher);
      }

      writer.Advance(packetSize);
    }
  }
}
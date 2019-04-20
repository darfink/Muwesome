using System;
using System.Buffers;
using System.IO.Pipelines;

namespace Muwesome.Packet.IO.Xor {
  public abstract class XorPipeline : PacketPipeReaderBase {
    private byte[] _cipher;

    /// <summary>Contructs a new XOR pipeline.</summary>
    public XorPipeline(byte[] xorCipher) => _cipher = xorCipher;

    /// <summary>Writes encrypted or decrypted packet data to a pipe.</summary>
    protected void ApplyCipherAndWrite(PipeWriter writer, ReadOnlySequence<byte> packet, bool encrypt) {
      int packetSize = (int)packet.Length;
      var output = writer.GetSpan(packetSize).Slice(0, packetSize);
      packet.CopyTo(output);

      var packetView = new PacketView(output);
      if (encrypt) {
        XorCipher.Encrypt(packetView, _cipher);
      } else {
        XorCipher.Decrypt(packetView, _cipher);
      }

      writer.Advance(packetSize);
    }
  }
}
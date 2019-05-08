using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Muwesome.Packet.Utility;

namespace Muwesome.Packet.IO.SimpleModulus {
  public class SimpleModulusPipelineDecryptor : SimpleModulusPipeline, IPipelineDecryptor {
    private readonly SimpleModulusKeys decryptionKeys;
    private readonly Pipe pipe = new Pipe();

    /// <summary>Initializes a new instance of the <see cref="SimpleModulusPipelineDecryptor"/> class.</summary>
    public SimpleModulusPipelineDecryptor(PipeReader source)
        : this(source, SimpleModulusKeys.DefaultClientKeys) {
    }

    /// <summary>Initializes a new instance of the <see cref="SimpleModulusPipelineDecryptor"/> class.</summary>
    public SimpleModulusPipelineDecryptor(PipeReader source, SimpleModulusKeys decryptionKeys) {
      this.Source = source;
      this.decryptionKeys = decryptionKeys;
      _ = this.ReadSource();
    }

    /// <inheritdoc/>
    public PipeReader Reader => this.pipe.Reader;

    /// <inheritdoc/>
    protected override void OnComplete(Exception exception) =>
      this.pipe.Writer.Complete(exception);

    /// <inheritdoc/>
    protected override async Task ReadPacket(ReadOnlySequence<byte> packet) {
      this.ProcessPacket(packet);
      await this.pipe.Writer.FlushAsync();
    }

    private void ProcessPacket(ReadOnlySequence<byte> packetData) {
      Span<byte> header = stackalloc byte[PacketView.MinimumSize];
      packetData.Slice(0, PacketView.MinimumSize).CopyTo(header);

      var packet = new PacketView(header);

      if (packet.Type.IsEncrypted) {
        if (packet.PayloadLength % EncryptedBlockSize != 0) {
          // TODO: Specialized exception
          throw new ArgumentException(nameof(packetData));
        }

        this.DecryptAndWrite(packet, packetData);
      } else {
        this.CopyAndWrite(this.pipe.Writer, packetData);
      }
    }

    private void DecryptAndWrite(PacketView packet, ReadOnlySequence<byte> packetData) {
      int maximumDecryptedSize = this.GetMaximumDecryptedSize(packet);
      var span = this.pipe.Writer.GetSpan(maximumDecryptedSize);

      // This is offset by one so the encryption counter is replaced with the unencrypted payload
      var decryptedHeaderSize = packet.Type.Decrypted.HeaderLength - 1;
      var decryptedData = span.Slice(0, maximumDecryptedSize);
      var decryptedPayloadSize = this.DecryptPacketPayload(
        input: packetData.Slice(packet.Type.HeaderLength),
        output: decryptedData.Slice(decryptedHeaderSize));
      var decryptedPacketSize = decryptedHeaderSize + decryptedPayloadSize;

      packet.Type.Decrypted.WriteHeader(decryptedData, decryptedPacketSize);
      this.pipe.Writer.Advance(decryptedPacketSize);
    }

    private int DecryptPacketPayload(ReadOnlySequence<byte> input, Span<byte> output) {
      Span<byte> inputBlock = stackalloc byte[EncryptedBlockSize];

      int decryptedSize = 0;
      for (var rest = input; rest.Length > 0; rest = rest.Slice(EncryptedBlockSize)) {
        rest.Slice(0, EncryptedBlockSize).CopyTo(inputBlock);
        var outputBlock = output.Slice(decryptedSize, DecryptedBlockSize);
        var decryptedBlockSize = this.DecryptBlock(inputBlock, outputBlock);

        if (decryptedSize == 0 && outputBlock[0] != this.Counter) {
          throw new ArgumentException(nameof(input));
        }

        decryptedSize += decryptedBlockSize;
      }

      this.Counter = (this.Counter + 1) % byte.MaxValue;
      return decryptedSize;
    }

    private int DecryptBlock(Span<byte> input, Span<byte> output) {
      for (int i = 0; i < this.EncryptionResult.Length; i++) {
        this.EncryptionResult[i] = this.ReadInputBuffer(input, i);
      }

      this.DecryptContent(output);
      return this.DecodeFooter(input, output);
    }

    private void DecryptContent(Span<byte> outputBlock) {
      for (int i = 2; i >= 0; i--) {
        this.EncryptionResult[i] ^= this.decryptionKeys.XorKey[i] ^ (this.EncryptionResult[i + 1] & 0xFFFF);
      }

      var output = MemoryMarshal.Cast<byte, ushort>(outputBlock);
      for (int i = 0; i < this.EncryptionResult.Length; i++) {
        var previousValue = i == 0 ? 0 : (this.EncryptionResult[i - 1] & 0xFFFF);
        var encryptMulMod = (this.EncryptionResult[i] * this.decryptionKeys.DecryptKey[i]) % this.decryptionKeys.ModulusKey[i];
        output[i] = (ushort)(this.decryptionKeys.XorKey[i] ^ encryptMulMod ^ previousValue);
      }
    }

    private uint ReadInputBuffer(Span<byte> inputBlock, int resultIndex) {
      var byteOffset = GetByteOffset(resultIndex);
      var bitOffset = GetBitOffset(resultIndex);
      var firstMask = GetFirstBitMask(resultIndex);
      uint result = 0;
      result += (uint)((inputBlock[byteOffset++] & firstMask) << (24 + bitOffset));
      result += (uint)(inputBlock[byteOffset++] << (16 + bitOffset));
      result += (uint)((inputBlock[byteOffset] & (0xFF << (8 - bitOffset))) << (8 + bitOffset));

      result = ReverseBytes(result);
      var remainderMask = GetRemainderBitMask(resultIndex);
      var remainder = (byte)(inputBlock[byteOffset] & remainderMask);
      int rightShift = 6 - bitOffset;
      result += (uint)(remainder << 16) >> rightShift;

      return result;
    }

    private int DecodeFooter(Span<byte> input, Span<byte> output) {
      var blockFooter = input.Slice(EncryptedBlockSize - 2, 2);

      byte blockSize = (byte)(blockFooter[0] ^ blockFooter[1] ^ BlockSizeXorKey);
      byte checksum = BlockCheckSumXorKey;
      for (int i = 0; i < DecryptedBlockSize; i++) {
        checksum ^= output[i];
      }

      if (blockFooter[1] != checksum) {
        throw new ArgumentException(nameof(input));
      }

      return blockSize;
    }

    private int GetMaximumDecryptedSize(PacketView packet) =>
      ((packet.PayloadLength / EncryptedBlockSize) * DecryptedBlockSize) + packet.Type.Decrypted.HeaderLength;
  }
}
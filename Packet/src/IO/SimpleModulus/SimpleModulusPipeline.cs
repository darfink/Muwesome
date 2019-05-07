using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;

namespace Muwesome.Packet.IO.SimpleModulus {
  public abstract class SimpleModulusPipeline : PacketPipeReaderBase {
    /// <summary>The size of a decrypted block.</summary>
    protected const int DecryptedBlockSize = 8;

    /// <summary>The size of an encrypted block.</summary>
    protected const int EncryptedBlockSize = 11;

    /// <summary>The XOR key used for encryption of the block size.</summary>
    protected const byte BlockSizeXorKey = 0x3D;

    /// <summary>The XOR key used for encryption of the block checksum.</summary>
    protected const byte BlockCheckSumXorKey = 0xF8;

    private const int BitsPerByte = 8;
    private const int BitsPerValue = (BitsPerByte * 2) + 2;

    /// <summary>Gets or sets the current encryption counter.</summary>
    public int Counter { get; protected set; }

    protected uint[] EncryptionResult { get; } = new uint[4];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static int GetByteOffset(int resultIndex) => GetBitIndex(resultIndex) / BitsPerByte;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static int GetBitOffset(int resultIndex) => GetBitIndex(resultIndex) % BitsPerByte;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static int GetFirstBitMask(int resultIndex) => 0xFF >> GetBitOffset(resultIndex);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static int GetRemainderBitMask(int resultIndex) => (0xFF << (6 - GetBitOffset(resultIndex)) & 0xFF) - ((0xFF << (8 - GetBitOffset(resultIndex))) & 0xFF);

    protected static uint ReverseBytes(uint value) {
      value = (value >> 16) | (value << 16);
      return ((value & 0xFF00FF00) >> 8) | ((value & 0x00FF00FF) << 8);
    }

    /// <summary>Writes a data sequence directly to the writer.</summary>
    protected void CopyAndWrite(PipeWriter writer, ReadOnlySequence<byte> packet) {
      int packetSize = (int)packet.Length;
      var output = writer.GetSpan(packetSize).Slice(0, packetSize);
      packet.CopyTo(output);
      writer.Advance(packetSize);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetBitIndex(int resultIndex) => resultIndex * BitsPerValue;
  }
}
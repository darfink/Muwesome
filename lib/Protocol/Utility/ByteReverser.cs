using System;

namespace Muwesome.Protocol.Utility {
  internal class ByteReverser :
      IByteReverser<ushort>,
      IByteReverser<short>,
      IByteReverser<ulong>,
      IByteReverser<long>,
      IByteReverser<uint>,
      IByteReverser<int> {
    public static readonly ByteReverser P = new ByteReverser();

    ushort IByteReverser<ushort>.ReverseBytes(ushort value) =>
      (ushort)((value & 0xFF) << 8 | ((value >> 8) & 0xFF));

    short IByteReverser<short>.ReverseBytes(short value) =>
      (short)ByteReverser<ushort>.P.ReverseBytes((ushort)value);

    uint IByteReverser<uint>.ReverseBytes(uint value) {
      value = (value >> 16) | (value << 16);
      return ((value & 0xFF00FF00) >> 8) | ((value & 0x00FF00FF) << 8);
    }

    int IByteReverser<int>.ReverseBytes(int value) =>
      (int)ByteReverser<uint>.P.ReverseBytes((uint)value);

    ulong IByteReverser<ulong>.ReverseBytes(ulong value) {
      value = (value >> 32) | (value << 32);
      value = ((value & 0xFFFF0000FFFF0000) >> 16) | ((value & 0x0000FFFF0000FFFF) << 16);
      return ((value & 0xFF00FF00FF00FF00) >> 8) | ((value & 0x00FF00FF00FF00FF) << 8);
    }

    long IByteReverser<long>.ReverseBytes(long value) =>
      (long)ByteReverser<ulong>.P.ReverseBytes((ulong)value);
  }
}
using System;

namespace Muwesome.Protocol.Utility {
  internal interface IByteReversible<T> {
    T ReverseBytes(T value);
  }

  internal class ByteReversible<T> : IByteReversible<T> {
    public static readonly IByteReversible<T> P = (ByteReversible.P as IByteReversible<T>) ?? new ByteReversible<T>();

    T IByteReversible<T>.ReverseBytes(T value) => throw new NotSupportedException();
  }

  internal class ByteReversible :
      IByteReversible<ushort>,
      IByteReversible<short>,
      IByteReversible<ulong>,
      IByteReversible<long>,
      IByteReversible<uint>,
      IByteReversible<int> {
    public static readonly ByteReversible P = new ByteReversible();

    ushort IByteReversible<ushort>.ReverseBytes(ushort value) =>
      (ushort)((value & 0xFF) << 8 | ((value >> 8) & 0xFF));

    short IByteReversible<short>.ReverseBytes(short value) =>
      (short)ByteReversible<ushort>.P.ReverseBytes((ushort)value);

    uint IByteReversible<uint>.ReverseBytes(uint value) {
      value = (value >> 16) | (value << 16);
      return ((value & 0xFF00FF00) >> 8) | ((value & 0x00FF00FF) << 8);
    }

    int IByteReversible<int>.ReverseBytes(int value) =>
      (int)ByteReversible<uint>.P.ReverseBytes((uint)value);

    ulong IByteReversible<ulong>.ReverseBytes(ulong value) {
      value = (value >> 32) | (value << 32);
      value = ((value & 0xFFFF0000FFFF0000) >> 16) | ((value & 0x0000FFFF0000FFFF) << 16);
      return ((value & 0xFF00FF00FF00FF00) >> 8) | ((value & 0x00FF00FF00FF00FF) << 8);
    }

    long IByteReversible<long>.ReverseBytes(long value) =>
      (long)ByteReversible<ulong>.P.ReverseBytes((ulong)value);
  }
}
using System;

namespace Muwesome.Protocol.Utility {
  internal interface IByteReverser<T> {
    /// <summary>Reverses the bytes of a number.</summary>
    T ReverseBytes(T value);
  }

  internal class ByteReverser<T> : IByteReverser<T> {
    public static readonly IByteReverser<T> Default = (ByteReverserImpl.Singleton as IByteReverser<T>) ?? new ByteReverser<T>();

    /// <inheritdoc />
    T IByteReverser<T>.ReverseBytes(T value) => throw new NotSupportedException();

    internal class ByteReverserImpl :
        IByteReverser<ushort>,
        IByteReverser<short>,
        IByteReverser<ulong>,
        IByteReverser<long>,
        IByteReverser<uint>,
        IByteReverser<int> {
      public static readonly ByteReverserImpl Singleton = new ByteReverserImpl();

      /// <inheritdoc />
      ushort IByteReverser<ushort>.ReverseBytes(ushort value) =>
        (ushort)((value & 0xFF) << 8 | ((value >> 8) & 0xFF));

      /// <inheritdoc />
      short IByteReverser<short>.ReverseBytes(short value) =>
        (short)ByteReverser<ushort>.Default.ReverseBytes((ushort)value);

      /// <inheritdoc />
      uint IByteReverser<uint>.ReverseBytes(uint value) {
        value = (value >> 16) | (value << 16);
        return ((value & 0xFF00FF00) >> 8) | ((value & 0x00FF00FF) << 8);
      }

      /// <inheritdoc />
      int IByteReverser<int>.ReverseBytes(int value) =>
        (int)ByteReverser<uint>.Default.ReverseBytes((uint)value);

      /// <inheritdoc />
      ulong IByteReverser<ulong>.ReverseBytes(ulong value) {
        value = (value >> 32) | (value << 32);
        value = ((value & 0xFFFF0000FFFF0000) >> 16) | ((value & 0x0000FFFF0000FFFF) << 16);
        return ((value & 0xFF00FF00FF00FF00) >> 8) | ((value & 0x00FF00FF00FF00FF) << 8);
      }

      /// <inheritdoc />
      long IByteReverser<long>.ReverseBytes(long value) =>
        (long)ByteReverser<ulong>.Default.ReverseBytes((ulong)value);
    }
  }
}
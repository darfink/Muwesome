using System;

namespace Muwesome.Protocol.Utility {
  internal interface IByteReverser<T> {
    T ReverseBytes(T value);
  }

  internal class ByteReverser<T> : IByteReverser<T> {
    public static readonly IByteReverser<T> P = (ByteReverser.P as IByteReverser<T>) ?? new ByteReverser<T>();

    T IByteReverser<T>.ReverseBytes(T value) => throw new NotSupportedException();
  }
}
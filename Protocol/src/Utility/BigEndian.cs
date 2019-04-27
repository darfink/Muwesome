using System;

namespace Muwesome.Protocol.Utility {
  public struct BigEndian<T> where T : struct {
    private T value;

    public static implicit operator BigEndian<T>(T rhs) =>
      new BigEndian<T> { value = BitConverter.IsLittleEndian ? ByteReversible<T>.P.ReverseBytes(rhs) : rhs };

    public static implicit operator T(BigEndian<T> rhs) =>
      BitConverter.IsLittleEndian ? ByteReversible<T>.P.ReverseBytes(rhs.value) : rhs.value;
  }
}
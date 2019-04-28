using System;

namespace Muwesome.Protocol.Utility {
  public struct LittleEndian<T>
      where T : struct {
    private T value;

    public static implicit operator LittleEndian<T>(T rhs) =>
      new LittleEndian<T> { value = BitConverter.IsLittleEndian ? rhs : ByteReverser<T>.P.ReverseBytes(rhs) };

    public static implicit operator T(LittleEndian<T> rhs) =>
      BitConverter.IsLittleEndian ? rhs.value : ByteReverser<T>.P.ReverseBytes(rhs.value);
  }
}
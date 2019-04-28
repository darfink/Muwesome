using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Muwesome.Packet {
  public static class ArrayExtensions {
    /// <summary>Reads a byte from the specified offset.</summary>
    public static byte ReadByte(this ReadOnlySpan<byte> bytes, int offset = 0) => bytes[offset];

    /// <summary>Reads a Big-endian UInt16 from the specified offset.</summary>
    public static ushort ReadUInt16BE(this ReadOnlySpan<byte> bytes, int offset = 0) {
      var value = MemoryMarshal.Read<ushort>(bytes.Slice(offset));
      return BitConverter.IsLittleEndian ? ReverseUInt16(value) : value;
    }

    /// <summary>Writes a byte at the specified offset.</summary>
    public static void WriteByte(this Span<byte> bytes, byte input, int offset = 0) => bytes[offset] = input;

    /// <summary>Writes a Big-endian UInt16 at the specified offset.</summary>
    public static void WriteUInt16BE(this Span<byte> bytes, ushort input, int offset = 0) {
      var value = BitConverter.IsLittleEndian ? ReverseUInt16(input) : input;
      MemoryMarshal.Write(bytes.Slice(offset), ref value);
    }

    /// <summary>Gets a byte array as a hex string.</summary>
    public static string AsHexString(this IEnumerable<byte> bytes) => string.Join(" ", bytes.Select(v => v.ToString("X2")));

    /// <summary>Gets a byte array as a hex string.</summary>
    public static string AsHexString(this Span<byte> bytes) => AsHexString((IEnumerable<byte>)bytes.ToArray());

    /// <summary>Reverses the bytes of a UInt16.</summary>
    private static ushort ReverseUInt16(ushort input) => (ushort)((input >> 8) | (input << 8));
  }
}
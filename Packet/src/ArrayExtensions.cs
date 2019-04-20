using System;
using System.Collections.Generic;
using System.Linq;

namespace Muwesome.Packet {
  public static class ArrayExtensions {
    public static byte ReadByte(this Span<byte> bytes, int offset = 0) => bytes[offset];

    public static ushort ReadUInt16LE(this Span<byte> bytes, int offset = 0) =>
      (ushort)(!BitConverter.IsLittleEndian
        ? (bytes[offset] << 8 | bytes[offset + 1] << 0)
        : (bytes[offset] << 0 | bytes[offset + 1] << 8));

    public static ushort ReadUInt16BE(this Span<byte> bytes, int offset = 0) =>
      (ushort)(BitConverter.IsLittleEndian
        ? (bytes[offset] << 8 | bytes[offset + 1] << 0)
        : (bytes[offset] << 0 | bytes[offset + 1] << 8));

    /// <summary>Gets a byte array as a hex string.</summary>
    public static string AsHexString(this IEnumerable<byte> bytes) =>
      string.Join(" ", bytes.Select(v => v.ToString("X")));
  }
}
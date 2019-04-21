using System;
using System.Collections.Generic;
using System.Text;
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

    public static string ReadString(this Span<byte> bytes, int maxLength, Encoding encoding, int offset = 0) {
      int bytesAvailable = Math.Min(maxLength, bytes.Length - offset);
      var data = bytes.Slice(offset, bytesAvailable).ToArray();

      int stringLength = data.TakeWhile(i => i != 0).Count();
      return encoding.GetString(data, 0, stringLength);
    }

    public static void WriteFixedString(this Span<byte> bytes, string text, int length, bool withNull, Encoding encoding, int offset = 0) {
      var data = encoding.GetBytes(text);
      int bytesRequired = data.Length + (withNull ? 1 : 0);

      if (bytesRequired > length) {
        throw new ArgumentOutOfRangeException(nameof(text));
      }

      var target = bytes.Slice(offset, length);
      target.Clear();
      data.AsSpan().CopyTo(target);
    }

    /// <summary>Gets a byte array as a hex string.</summary>
    public static string AsHexString(this IEnumerable<byte> bytes) =>
      string.Join(" ", bytes.Select(v => v.ToString("X")));
  }
}
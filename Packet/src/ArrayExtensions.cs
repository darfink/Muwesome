using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Muwesome.Packet {
  public static class ArrayExtensions {
    /// <summary>Reads a byte from the specified offset.</summary>
    public static byte ReadByte(this Span<byte> bytes, int offset = 0) => bytes[offset];

    /// <summary>Reads a Little-endian UInt16 from the specified offset.</summary>
    public static ushort ReadUInt16LE(this Span<byte> bytes, int offset = 0) {
      var value = MemoryMarshal.Read<ushort>(bytes.Slice(offset));
      return BitConverter.IsLittleEndian ? value : ReverseUInt16(value);
    }

    /// <summary>Reads a Big-endian UInt16 from the specified offset.</summary>
    public static ushort ReadUInt16BE(this Span<byte> bytes, int offset = 0) {
      var value = MemoryMarshal.Read<ushort>(bytes.Slice(offset));
      return BitConverter.IsLittleEndian ? ReverseUInt16(value) : value;
    }

    /// <summary>Reads a string from the specified offset.</summary>
    public static string ReadString(this Span<byte> bytes, int maxLength, Encoding encoding, int offset = 0) {
      int bytesAvailable = Math.Min(maxLength, bytes.Length - offset);
      var data = bytes.Slice(offset, bytesAvailable).ToArray();

      int stringLength = data.TakeWhile(i => i != 0).Count();
      return encoding.GetString(data, 0, stringLength);
    }

    /// <summary>Writes a byte at the specified offset.</summary>
    public static void WriteByte(this Span<byte> bytes, byte input, int offset = 0) => bytes[offset] = input;

    /// <summary>Writes a Little-endian UInt16 at the specified offset.</summary>
    public static void WriteUInt16LE(this Span<byte> bytes, ushort input, int offset = 0) {
      var value = BitConverter.IsLittleEndian ? input : ReverseUInt16(input);
      MemoryMarshal.Write(bytes.Slice(offset), ref value);
    }

    /// <summary>Writes a Big-endian UInt16 at the specified offset.</summary>
    public static void WriteUInt16BE(this Span<byte> bytes, ushort input, int offset = 0) {
      var value = BitConverter.IsLittleEndian ? ReverseUInt16(input) : input;
      MemoryMarshal.Write(bytes.Slice(offset), ref value);
    }

    /// <summary>Writes a fixed string at the specified offset.</summary>
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
    public static string AsHexString(this IEnumerable<byte> bytes) => string.Join(" ", bytes.Select(v => v.ToString("X")));

    /// <summary>Reverses the bytes of a UInt16.</summary>
    private static ushort ReverseUInt16(ushort input) => (ushort)((input >> 8) | (input << 8));
  }
}
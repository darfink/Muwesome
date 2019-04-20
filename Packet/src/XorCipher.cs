using System;

namespace Muwesome.Packet {
  public static class XorCipher {
    /// <summary>The length of an XOR cipher.</summary>
    public const int RequiredLength = 32;

    /// <summary>The default XOR cipher keys.</summary>
    public static readonly byte[] DefaultKeys = new byte[RequiredLength] {
      0xE7, 0x6D, 0x3A, 0x89, 0xBC, 0xB2, 0x9F, 0x73, 0x23, 0xA8, 0xFE, 0xB6, 0x49, 0x5D, 0x39, 0x5D,
      0x8A, 0xCB, 0x63, 0x8D, 0xEA, 0x7D, 0x2B, 0x5F, 0xC3, 0xB1, 0xE9, 0x83, 0x29, 0x51, 0xE8, 0x56,
    };

    /// <summary>Encrypts a packet with an XOR cipher.</summary>
    public static void Encrypt(PacketView packet, byte[] cipher) =>
      ApplyCipher(packet, cipher, encrypt: true);

    /// <summary>Decrypt a packet with an XOR cipher.</summary>
    public static void Decrypt(PacketView packet, byte[] cipher) =>
      ApplyCipher(packet, cipher, encrypt: false);

    private static void ApplyCipher(PacketView packet, byte[] cipher, bool encrypt) {
      if (cipher.Length != RequiredLength) {
        throw new ArgumentException($"The cipher size must be {RequiredLength}, but is {cipher.Length}", nameof(cipher));
      }

      if (packet.IsPartial) {
        throw new ArgumentException("Refusing to apply XOR cipher to partial packet", nameof(packet));
      }

      if (packet.IsEncrypted) {
        throw new ArgumentException("Refusing to apply XOR cipher to C3/C4 packet", nameof(packet));
      }

      int start = encrypt ? packet.HeaderLength + 1 : packet.Length - 1;
      int end = encrypt ? packet.Length : packet.HeaderLength;

      for (int i = start; i != end; i += (encrypt ? 1 : -1)) {
        packet.Data[i] ^= (byte)(packet.Data[i - 1] ^ cipher[i % cipher.Length]);
      }
    }
  }
}
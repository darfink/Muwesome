using System.Linq;

namespace Muwesome.Packet.IO.SimpleModulus {
  public class SimpleModulusKeys {
    private const int NumbersPerKey = 4;

    public uint[] ModulusKey { get; } = { 0, 0, 0, 0 };

    public uint[] XorKey { get; } = { 0, 0, 0, 0 };

    public uint[] EncryptKey { get; } = { 0, 0, 0, 0 };

    public uint[] DecryptKey { get; } = { 0, 0, 0, 0 };

    public static SimpleModulusKeys CreateDecryptionKeys(uint[] decryptionKey) {
      var keys = new SimpleModulusKeys();

      uint[][] entries = new uint[][] { keys.ModulusKey, keys.DecryptKey, keys.XorKey };
      for (int i = 0; i < NumbersPerKey * entries.Length; i++) {
        entries[i / NumbersPerKey][i % NumbersPerKey] = decryptionKey[i];
      }

      return keys;
    }

    public static SimpleModulusKeys CreateEncryptionKeys(uint[] encryptionKey) {
      var keys = new SimpleModulusKeys();

      uint[][] entries = new uint[][] { keys.ModulusKey, keys.EncryptKey, keys.XorKey };
      for (int i = 0; i < NumbersPerKey * entries.Length; i++) {
        entries[i / NumbersPerKey][i % NumbersPerKey] = encryptionKey[i];
      }

      return keys;
    }
  }
}
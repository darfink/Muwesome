using System;
using System.Linq;

namespace Muwesome.Packet.IO.SimpleModulus {
  public class SimpleModulusKeys {
    /// <summary>Default keys for server encryption and server decryption.</summary>
    public static readonly SimpleModulusKeys DefaultServerKeys = new SimpleModulusKeys(
      modKey: new uint[] { 73326, 109989, 98843, 171058 },
      xorKey: new uint[] { 62004, 64409, 35374, 64599 },
      encKey: new uint[] { 13169, 19036, 35482, 29587 },
      decKey: new uint[] { 18035, 30340, 24701, 11141 });

    /// <summary>Default keys for client encryption and client decryption.</summary>
    public static readonly SimpleModulusKeys DefaultClientKeys = new SimpleModulusKeys(
      modKey: new uint[] { 128079, 164742, 70235, 106898 },
      xorKey: new uint[] { 48413, 46165, 15171, 37433 },
      encKey: new uint[] { 23489, 11911, 19816, 13647 },
      decKey: new uint[] { 31544, 2047, 57011, 10183 });

    /// <summary>Initializes a new instance of the <see cref="SimpleModulusKeys"/> class.</summary>
    public SimpleModulusKeys(uint[] modKey, uint[] xorKey, uint[] encKey, uint[] decKey) {
      Array.Copy(modKey, this.ModulusKey, this.ModulusKey.Length);
      Array.Copy(encKey, this.EncryptKey, this.EncryptKey.Length);
      Array.Copy(decKey, this.DecryptKey, this.DecryptKey.Length);
      Array.Copy(xorKey, this.XorKey, this.XorKey.Length);
    }

    /// <summary>Gets the keys used for modulus operations.</summary>
    public uint[] ModulusKey { get; } = { 0, 0, 0, 0 };

    /// <summary>Gets the keys used for XOR operations.</summary>
    public uint[] XorKey { get; } = { 0, 0, 0, 0 };

    /// <summary>Gets the encryption keys used for multiplicative operations.</summary>
    public uint[] EncryptKey { get; } = { 0, 0, 0, 0 };

    /// <summary>Gets the decryption keys used for multiplicative operations.</summary>
    public uint[] DecryptKey { get; } = { 0, 0, 0, 0 };
  }
}
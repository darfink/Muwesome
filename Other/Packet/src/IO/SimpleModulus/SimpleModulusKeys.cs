using System;
using System.Collections.Generic;
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

    /// <summary>The number of values for each key.</summary>
    private const int ValuesPerKey = 4;

    /// <summary>Initializes a new instance of the <see cref="SimpleModulusKeys"/> class.</summary>
    public SimpleModulusKeys(uint[] modKey, uint[] xorKey, uint[] encKey, uint[] decKey) {
      if (modKey.Length != ValuesPerKey) {
        throw new ArgumentOutOfRangeException(nameof(modKey));
      }

      if (xorKey.Length != ValuesPerKey) {
        throw new ArgumentOutOfRangeException(nameof(xorKey));
      }

      if (encKey.Length != ValuesPerKey) {
        throw new ArgumentOutOfRangeException(nameof(encKey));
      }

      if (decKey.Length != ValuesPerKey) {
        throw new ArgumentOutOfRangeException(nameof(decKey));
      }

      this.ModulusKey = modKey;
      this.EncryptKey = encKey;
      this.DecryptKey = decKey;
      this.XorKey = xorKey;
    }

    /// <summary>Gets the keys used for modulus operations.</summary>
    public IReadOnlyList<uint> ModulusKey { get; private set; }

    /// <summary>Gets the keys used for XOR operations.</summary>
    public IReadOnlyList<uint> XorKey { get; private set; }

    /// <summary>Gets the encryption keys used for multiplicative operations.</summary>
    public IReadOnlyList<uint> EncryptKey { get; private set; }

    /// <summary>Gets the decryption keys used for multiplicative operations.</summary>
    public IReadOnlyList<uint> DecryptKey { get; private set; }
  }
}
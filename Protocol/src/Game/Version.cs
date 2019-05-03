using System;
using System.Runtime.InteropServices;

namespace Muwesome.Protocol.Game {
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct Version {
    public static readonly Version V10203 = new Version(1, 2, 3);

    /// <summary>Initializes a new instance of the <see cref="Version"/> struct.</summary>
    public Version(byte major, byte minor, byte patch) {
      this.Major = major;
      this.Minor = minor;
      this.Patch = patch;
    }

    /// <summary>Gets the major version.</summary>
    public byte Major { get; set; }

    /// <summary>Gets the minor version.</summary>
    public byte Minor { get; set; }

    /// <summary>Gets the patch version.</summary>
    public byte Patch { get; set; }

    /// <inheritdoc />
    public override string ToString() => $"{Major}.{Minor}.{(char)('A' + Patch)}";
  }
}
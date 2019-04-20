
namespace Muwesome.Protocol.Connect {
  public struct Version {
    public static readonly Version V1_02_C_EN = new Version(1, 2, 3);

    public Version(byte major, byte minor, byte patch) {
      Major = major;
      Minor = minor;
      Patch = patch;
    }

    public byte Major { get; }
    public byte Minor { get; }
    public byte Patch { get; }
  }
}
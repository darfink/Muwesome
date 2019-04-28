namespace Muwesome.Protocol.Game {
  public class Version {
    public static readonly Version V010203 = new Version(1, 2, 3);

    public Version(byte major, byte minor, byte patch) {
      this.Major = major;
      this.Minor = minor;
      this.Patch = patch;
    }

    public byte Major { get; }

    public byte Minor { get; }

    public byte Patch { get; }
  }
}
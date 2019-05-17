using System;
using System.Runtime.InteropServices;

namespace Muwesome.Protocol.Game {
  public struct ClientVersion : IEquatable<ClientVersion>, IComparable<ClientVersion>, IComparable {
    public static readonly ClientVersion V10203 = new ClientVersion(1, 2, 3);

    /// <summary>Initializes a new instance of the <see cref="ClientVersion"/> struct.</summary>
    public ClientVersion(byte major, byte minor, byte patch) {
      this.Major = major;
      this.Minor = minor;
      this.Patch = patch;
    }

    /// <summary>Gets the major version.</summary>
    public byte Major { get; }

    /// <summary>Gets the minor version.</summary>
    public byte Minor { get; }

    /// <summary>Gets the patch version.</summary>
    public byte Patch { get; }

    /// <summary>Equality compares this version to another one.</summary>
    public static bool operator ==(ClientVersion left, ClientVersion right) => Equals(left, right);

    /// <summary>Inequality compares this version to another one.</summary>
    public static bool operator !=(ClientVersion left, ClientVersion right) => !Equals(left, right);

    /// <inheritdoc />
    public int CompareTo(object other) {
      if (other is ClientVersion version) {
        return this.CompareTo(version);
      }

      throw new ArgumentException($"{nameof(Object)} is not a {nameof(ClientVersion)}");
    }

    /// <inheritdoc />
    public int CompareTo(ClientVersion other) {
      int major = this.Major.CompareTo(other.Major);
      int minor = this.Minor.CompareTo(other.Minor);
      int patch = this.Patch.CompareTo(other.Patch);

      return major != 0 ? major : (minor != 0 ? minor : patch);
    }

    /// <inheritdoc />
    public override int GetHashCode() => (this.Major, this.Minor, this.Patch).GetHashCode();

    /// <inheritdoc />
    public override bool Equals(object other) => other is ClientVersion version && this.Equals(version);

    /// <inheritdoc />
    public bool Equals(ClientVersion other) => this.CompareTo(other) == 0;

    /// <inheritdoc />
    public override string ToString() => $"{this.Major}.{this.Minor}.{(char)('@' + this.Patch)}";

    /// <summary>Creates a client version from bytes.</summary>
    internal static ClientVersion FromFiveBytes(Span<byte> bytes) {
      if (bytes.Length < 5) {
        throw new ArgumentOutOfRangeException(nameof(bytes));
      }

      return new ClientVersion(
        major: (byte)(bytes[0] - '0'),
        minor: (byte)(((bytes[1] - '0') * 10) + (bytes[2] - '0')),
        patch: (byte)(((bytes[3] - '0') * 10) + (bytes[4] - '0')));
    }

    /// <summary>Copies the client version to a span.</summary>
    internal void CopyToFiveBytes(Span<byte> span) {
      if (span.Length < 5) {
        throw new ArgumentOutOfRangeException(nameof(span));
      }

      span[0] = (byte)(this.Major + '0');
      span[1] = (byte)((this.Minor / 10) + '0');
      span[2] = (byte)((this.Minor % 10) + '0');
      span[3] = (byte)((this.Patch / 10) + '0');
      span[4] = (byte)((this.Patch % 10) + '0');
    }
  }
}
namespace Muwesome.Packet {
  /// <summary>Represents a packet.</summary>
  public interface IPacket {
  }

  /// <summary>Represents a fixed sized packet.</summary>
  public interface IFixedPacket : IPacket {
  }

  /// <summary>Represents a dynamically sized packet.</summary>
  public interface IDynamicPacket<T> : IPacket {
    /// <summary>Gets or sets the number of entries.</summary>
    int Count { get; set; }
  }
}
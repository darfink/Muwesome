namespace Muwesome.Protocol {
  public interface IPacket { }

  public interface IFixedPacket : IPacket { }

  public interface IDynamicPacket<T> : IPacket {
    int Count { get; set; }
  }
}
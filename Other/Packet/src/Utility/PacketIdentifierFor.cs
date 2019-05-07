using Muwesome.Packet;

namespace Muwesome.Packet.Utility {
  public static class PacketIdentifierFor<T>
      where T : IPacket {
    private static readonly PacketAttribute Attribute;

    /// <summary>Initializes static members of the <see cref="PacketIdentifierFor{T}" /> class.</summary>
    static PacketIdentifierFor() =>
      Attribute = (PacketAttribute)System.Attribute.GetCustomAttribute(typeof(T), typeof(PacketAttribute), false);

    /// <summary>Gets the packet's identifier.</summary>
    public static PacketIdentifier Identifier => Attribute.Identifier;
  }
}
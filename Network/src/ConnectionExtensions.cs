using Muwesome.Packet;

namespace Muwesome.Network {
  public static class ConnectionExtensions {
    /// <summary>Initiates a fixed sized packet transaction.</summary>
    public static ThreadSafeWriter SendPacket<TPacket>(this IConnection connection)
      where TPacket : IFixedSizedPacket => ThreadSafeWriter.CreateWith<TPacket>(connection);

    /// <summary>Initiates a variable sized packet transaction.</summary>
    public static ThreadSafeWriter SendPacket<TPacket>(this IConnection connection, int packetSize)
      where TPacket : IVariableSizedPacket => ThreadSafeWriter.CreateWith<TPacket>(connection, packetSize);
  }
}
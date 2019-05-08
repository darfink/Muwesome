using Muwesome.Packet;
using Muwesome.Packet.Utility;

namespace Muwesome.Network {
  public static class ConnectionExtensions {
    /// <summary>Initiates a thread-safe write transaction.</summary>
    public static WriteTransaction StartWrite(this IConnection connection, int payloadSize) =>
      new WriteTransaction(connection, payloadSize);

    /// <summary>Initiates a packet write.</summary>
    public static PacketWriteTransaction<T> SendPacket<T>(this IConnection connection)
        where T : struct, IFixedPacket =>
      new PacketWriteTransaction<T>(new WriteTransaction(connection, PacketHelper.GetPacketSize<T>()));
  }
}
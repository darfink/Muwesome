using Muwesome.Packet;
using Muwesome.Packet.Utility;

namespace Muwesome.Network {
  public static class ConnectionExtensions {
    /// <summary>Initiates a thread-safe transaction.</summary>
    public static ThreadSafeWriter StartWrite(this IConnection connection, int payloadSize) =>
      new ThreadSafeWriter(connection, payloadSize);

    /// <summary>Initiates a packet write.</summary>
    public static PacketWriter<T> SendPacket<T>(this IConnection connection) where T : struct, IFixedPacket =>
      new PacketWriter<T>(new ThreadSafeWriter(connection, PacketHelper.GetPacketSize<T>()));
  }
}
using Muwesome.Packet;

namespace Muwesome.Network {
  public static class ConnectionExtensions {
    /// <summary>Initiates a thread-safe transaction.</summary>
    public static ThreadSafeWriter StartWrite(this IConnection connection, int payloadSize) =>
      new ThreadSafeWriter(connection, payloadSize);
  }
}
namespace Muwesome.Packet.IO {
  public class MaxPacketSizeExceededException : InvalidPacketException {
    /// <summary>Initializes a new instance of the <see cref="MaxPacketSizeExceededException"/> class.</summary>
    public MaxPacketSizeExceededException(byte[] packet, int size, int maxSize)
        : base(packet, $"The packet size is {size}; may not exceed {maxSize} bytes") {
    }
  }
}
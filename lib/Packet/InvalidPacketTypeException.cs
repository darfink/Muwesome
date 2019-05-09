using Muwesome.Packet.Utility;

namespace Muwesome.Packet {
  public class InvalidPacketTypeException : InvalidPacketException {
    /// <summary>Initializes a new instance of the <see cref="InvalidPacketTypeException"/> class.</summary>
    public InvalidPacketTypeException(byte[] packet)
        : base(packet, $"The packet type is {packet[0]}; must be one of {PacketType.ValidTypes.ToHexString()}") {
    }
  }
}
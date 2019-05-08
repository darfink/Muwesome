using System;
using System.Text;
using Muwesome.Packet.Utility;

namespace Muwesome.Packet {
  public class InvalidPacketSizeException : InvalidPacketException {
    /// <summary>Initializes a new instance of the <see cref="InvalidPacketSizeException"/> class.</summary>
    public InvalidPacketSizeException(byte[] packet, int size)
        : base(packet, $"The packet size is {size}; must be at least {PacketView.MinimumSize} bytes") {
    }
  }
}
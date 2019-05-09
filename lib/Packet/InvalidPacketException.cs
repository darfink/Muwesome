using System;
using System.Text;
using Muwesome.Packet.Utility;

namespace Muwesome.Packet {
  public class InvalidPacketException : Exception {
    private string message;

    /// <summary>Initializes a new instance of the <see cref="InvalidPacketException"/> class.</summary>
    public InvalidPacketException(byte[] packetData, string cause = null) {
      this.Cause = cause ?? "The packet is invalid";
      this.PacketData = packetData;
    }

    /// <summary>Gets the invalid packet.</summary>
    public byte[] PacketData { get; private set; }

    /// <summary>Gets a message that describes the exception.</summary>
    public override string Message => this.message ?? (this.message = this.BuildMessage());

    /// <summary>Gets a message that describes the cause.</summary>
    public string Cause { get; private set; }

    private string BuildMessage() =>
      new StringBuilder()
        .AppendLine(this.Cause)
        .Append("Packet: ").AppendLine(this.PacketData.ToHexString())
        .ToString();
  }
}
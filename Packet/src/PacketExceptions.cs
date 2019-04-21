using System;
using System.Text;

namespace Muwesome.Packet {
  public class InvalidPacketException : Exception {
    private string _message;

    /// <summary>Constructs a new invalid packet exception.</summary>
    public InvalidPacketException(byte[] packetData, string cause = null) {
      Cause = cause ?? "The packet is invalid";
      PacketData = packetData;
    }

    /// <summary>Gets the invalid packet.</summary>
    public byte[] PacketData { get; private set; }

    /// <summary>Gets a message that describes the exception.</summary>
    public override string Message => _message ?? (_message = BuildMessage());

    /// <summary>Gets a message that describes the cause.</summary>
    public string Cause { get; private set; }

    private string BuildMessage() =>
      new StringBuilder()
        .AppendLine(Cause)
        .Append("Packet: ").AppendLine(PacketData.AsHexString())
        .ToString();
  }

  public class InvalidPacketTypeException : InvalidPacketException {
    /// <summary>Constructs a new <see cref="InvalidPacketTypeException" />.</summary>
    public InvalidPacketTypeException(byte[] packet) :
      base(packet, $"The packet type is {packet[0]}; must be one of {PacketType.ValidTypes.AsHexString()}") { }
  }

  public class InvalidPacketSizeException : InvalidPacketException {
    /// <summary>Constructs a new <see cref="InvalidPacketSizeException" />.</summary>
    public InvalidPacketSizeException(byte[] packet, int size) :
      base(packet, $"The packet size is {size}; must be at least {PacketView.MinimumSize} bytes") { }
  }

  public class MaxPacketSizeExceededException : InvalidPacketException {
    /// <summary>Constructs a new <see cref="MaxPacketSizeExceededException" />.</summary>
    public MaxPacketSizeExceededException(byte[] packet, int size, int maxSize) :
      base(packet, $"The packet size is {size}; may not exceed {maxSize} bytes") { }
  }
}
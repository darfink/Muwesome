using System;
using System.Text;
using Muwesome.Packet;

namespace Muwesome.Protocol {
  public class ConflictingPacketHandlersException : Exception {
    /// <summary>Constructs a new <see cref="ConflictingPacketHandlersException" />.</summary>
    private ConflictingPacketHandlersException(string message) : base(message) { }

    /// <summary>Constructs a new <see cref="ConflictingPacketHandlersException" />.</summary>
    public static ConflictingPacketHandlersException WithPacket<TPacket>() {
      var message = new StringBuilder()
        .AppendLine("Conflicting packet handlers")
        .Append("Packet: ").AppendLine(PacketFor<TPacket>.Name)
        .Append("Identifier: ").AppendLine(PacketFor<TPacket>.Identifier.AsHexString());
      return new ConflictingPacketHandlersException(message.ToString());
    }
  }
}
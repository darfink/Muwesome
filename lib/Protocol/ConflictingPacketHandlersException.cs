using System;
using System.Text;
using Muwesome.Packet;
using Muwesome.Packet.Utility;

namespace Muwesome.Protocol {
  public class ConflictingPacketHandlersException : Exception {
    /// <summary>Initializes a new instance of the <see cref="ConflictingPacketHandlersException"/> class.</summary>
    public ConflictingPacketHandlersException(PacketIdentifier packet)
        : base(BuildMessage(packet)) {
    }

    private static string BuildMessage(PacketIdentifier packet) =>
      new StringBuilder()
        .AppendLine("Conflicting packet handlers")
        .Append("Packet: ").AppendLine(packet.Name)
        .Append("Identifier: ").AppendLine(packet.Identifier.ToHexString())
        .ToString();
  }
}
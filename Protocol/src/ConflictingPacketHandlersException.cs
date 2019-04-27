using System;
using System.Text;
using Muwesome.Packet;

namespace Muwesome.Protocol {
  public class ConflictingPacketHandlersException : Exception {
    /// <summary>Constructs a new <see cref="ConflictingPacketHandlersException" />.</summary>
    public ConflictingPacketHandlersException(PacketIdentifier packet) : base(BuildMessage(packet)) { }

    private static string BuildMessage(PacketIdentifier packet) =>
      new StringBuilder()
        .AppendLine("Conflicting packet handlers")
        .Append("Packet: ").AppendLine(packet.Name)
        .Append("Identifier: ").AppendLine(packet.Identifier.AsHexString())
        .ToString();
  }
}
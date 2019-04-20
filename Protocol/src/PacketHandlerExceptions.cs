using System;
using System.Text;
using Muwesome.Packet;

namespace Muwesome.Protocol {
  public abstract class PacketHandlerException : Exception {
    private string _message;

    /// <summary>Gets a message that describes the exception.</summary>
    public override string Message => _message ?? (_message = BuildMessage());

    /// <summary>Builds a message of the exception.</summary>
    protected abstract string BuildMessage();
  }

  public class ConflictingPacketHandlersException : PacketHandlerException {
    /// <summary>Constructs a new invalid packet exception.</summary>
    public ConflictingPacketHandlersException(PacketDefinition packetDefinition) =>
      PacketDefinition = packetDefinition;

    /// <summary>Gets the packet with conflicts.</summary>
    public PacketDefinition PacketDefinition { get; private set; }

    /// <inheritdoc />
    protected override string BuildMessage() =>
      new StringBuilder()
        .AppendLine("Conflicting packet handlers")
        .Append("Packet: ").AppendLine(PacketDefinition.GetType().Name)
        .Append("Identifier: ").AppendLine(PacketDefinition.Identifier.AsHexString())
        .ToString();
  }
}
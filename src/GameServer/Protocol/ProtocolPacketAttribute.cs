using System;
using Muwesome.Packet;

namespace Muwesome.GameServer.Protocol {
  [AttributeUsage(AttributeTargets.Class)]
  public class ProtocolPacketAttribute : Attribute {
    /// <summary>Initializes a new instance of the <see cref="ProtocolPacketAttribute"/> class.</summary>
    public ProtocolPacketAttribute(Type packet) => this.Packet = PacketIdentifier.Get(packet);

    /// <summary>Gets the protocol packet identifier.</summary>
    public PacketIdentifier Packet { get; }

    /// <summary>Gets a type's protocol packet attribute.</summary>
    public static ProtocolPacketAttribute Get(Type type) =>
      (ProtocolPacketAttribute)System.Attribute.GetCustomAttribute(type, typeof(ProtocolPacketAttribute), false);
  }
}
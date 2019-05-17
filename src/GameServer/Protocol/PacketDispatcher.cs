using System;
using Muwesome.GameLogic.Actions;
using Muwesome.Packet;
using Muwesome.Packet.Utility;
using Muwesome.Protocol;

namespace Muwesome.GameServer.Protocol {
  /// <summary>A packet dispatcher for clients.</summary>
  internal abstract class PacketDispatcher<TPacket, TAction> : IActionProvider<Client>
      where TPacket : IPacket
      where TAction : Delegate {
    /// <summary>Gets the type of packet this instance dispatches.</summary>
    public PacketIdentifier Identifier => PacketIdentifierFor<TPacket>.Identifier;

    /// <inheritdoc />
    public Delegate CreateAction(Client client) => this.CreateDispatcherForAction(client);

    /// <summary>Creates a <see cref="TAction" /> packet dispatcher bound to a client.</summary>
    protected abstract TAction CreateDispatcherForAction(Client client);
  }
}
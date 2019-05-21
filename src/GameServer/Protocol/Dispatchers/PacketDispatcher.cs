using System;
using System.Linq;
using Muwesome.GameLogic.Actions;
using Muwesome.MethodDelegate.Extensions;
using Muwesome.Packet;
using Muwesome.Packet.Utility;
using Muwesome.Protocol;

namespace Muwesome.GameServer.Protocol.Dispatchers {
  /// <summary>A packet dispatcher for clients.</summary>
  internal abstract class PacketDispatcher<TPacket> : IActionProvider<Client>
      where TPacket : IPacket {
    /// <summary>Gets the type of packet this instance dispatches.</summary>
    public PacketIdentifier Identifier => PacketIdentifierFor<TPacket>.Identifier;

    /// <summary>Creates an action associated with a context.</summary>
    public Delegate CreateAction(Client client) => this.GetMethodDelegates(_ => client).First();
  }
}
using System;
using System.Collections.Generic;
using Muwesome.GameLogic.Actions;

namespace Muwesome.GameServer.Protocol {
  /// <summary>A client packet dispatcher.</summary>
  internal class ClientPacketDispatcher {
    private readonly List<IPlayerActionProvider> dispatchers = new List<IPlayerActionProvider>();

    /// <summary>Gets the packet dispatcher actions.</summary>
    public IReadOnlyCollection<IPlayerActionProvider> Actions => this.dispatchers;

    /// <summary>Registers a client packet dispatcher.</summary>
    public void Register<TAction>(IPlayerActionProvider<TAction> packetDispatcher)
        where TAction : Delegate => this.dispatchers.Add(packetDispatcher.AsGeneric());
  }
}
using System;
using System.Collections.Generic;
using Muwesome.GameLogic.Actions;

namespace Muwesome.GameServer.Protocol {
  /// <summary>A client packet dispatcher.</summary>
  public class ClientPacketDispatcher {
    private readonly List<IPlayerActionFactory> dispatchers = new List<IPlayerActionFactory>();

    /// <summary>Gets the packet dispatcher actions.</summary>
    public IReadOnlyCollection<IPlayerActionFactory> Actions => this.dispatchers;

    /// <summary>Registers a client packet dispatcher.</summary>
    public void Register<TAction>(IPlayerActionFactory<TAction> packetDispatcher)
        where TAction : Delegate => this.dispatchers.Add(packetDispatcher.AsGeneric());
  }
}
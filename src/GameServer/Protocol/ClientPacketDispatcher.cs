using System;
using System.Collections.Generic;
using Muwesome.GameLogic.Actions;
using Muwesome.Packet;

namespace Muwesome.GameServer.Protocol {
  /// <summary>A client packet dispatcher.</summary>
  internal class ClientPacketDispatcher {
    private readonly IList<IActionProvider<Client>> actionProviders = new List<IActionProvider<Client>>();

    public void RegisterActions(Client client, Action<Delegate> registerAction) {
      foreach (var actionProvider in this.actionProviders) {
        registerAction(actionProvider.CreateAction(client));
      }
    }

    /// <summary>Registers a new handler for a packet.</summary>
    // TODO: Validate unique packet and action?
    public void RegisterDispatcher<TPacket, TAction>(PacketDispatcher<TPacket, TAction> packetDispatcher)
        where TPacket : IPacket
        where TAction : Delegate =>
      this.actionProviders.Add(packetDispatcher);
  }
}
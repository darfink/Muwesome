using System;
using Muwesome.GameLogic;
using Muwesome.GameLogic.Actions;

namespace Muwesome.GameServer.Protocol {
  /// <summary>A packet dispatcher for join results.</summary>
  internal abstract class PacketDispatcher<TAction> : IPlayerActionProvider<TAction>
      where TAction : Delegate {
    /// <summary>Initializes a new instance of the <see cref="PacketDispatcher{T}"/> class.</summary>
    public PacketDispatcher(IClientController clientController) =>
      this.ClientController = clientController;

    /// <summary>Gets the client controller.</summary>
    protected IClientController ClientController { get; private set; }

    /// <inheritdoc />
    public TAction CreateAction(Player player) =>
      this.CreateAction(this.ClientController.GetClientByPlayer(player));

    /// <summary>Creates an action associated with the client.</summary>
    protected abstract TAction CreateAction(Client client);
  }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Muwesome.DomainModel.Entities;
using Muwesome.GameLogic.Interface;
using Muwesome.Persistence;

namespace Muwesome.GameLogic {
  /// <summary>Represents a player in-game.</summary>
  public class Player : IIdentifiable, IDisposable {
    private readonly IActionSet actions;

    /// <summary>Initializes a new instance of the <see cref="Player"/> class.</summary>
    internal Player(IAccountContext persistenceContext, IActionSet actions) {
      this.PersistenceContext = persistenceContext;
      this.actions = actions;
    }

    /// <summary>An event that is raised when the player is removed from the game.</summary>
    public event EventHandler Disposed;

    /// <inheritdoc />
    public ushort Id { get; set; }

    /// <summary>Gets or sets the account.</summary>
    public Account Account { get; set; }

    /// <summary>Gets or sets the persistence context.</summary>
    public IAccountContext PersistenceContext { get; set; }

    /// <summary>Gets an invokable action.</summary>
    public T Action<T>()
        where T : Delegate => this.actions.Get<T>();

    /// <inheritdoc />
    public void Dispose() {
      this.Disposed?.Invoke(this, EventArgs.Empty);
    }
  }
}
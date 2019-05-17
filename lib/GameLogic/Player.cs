using System;
using System.Collections.Generic;
using System.Linq;
using Muwesome.GameLogic.Actions;

namespace Muwesome.GameLogic {
  /// <summary>Represents a player in-game.</summary>
  public class Player : IIdentifiable, IDisposable {
    /// <summary>Initializes a new instance of the <see cref="Player"/> class.</summary>
    internal Player() {
    }

    /// <summary>An event that is raised when the player is removed from the game.</summary>
    public event EventHandler Disposed;

    /// <inheritdoc />
    public ushort Id { get; set; }

    /// <summary>Gets or sets the defined actions.</summary>
    internal ActionBag Actions { get; set; }

    /// <summary>Gets an invokable action.</summary>
    public T Action<T>()
        where T : Delegate => this.Actions.Get<T>();

    /// <inheritdoc />
    public void Dispose() {
      this.Disposed?.Invoke(this, EventArgs.Empty);
    }
  }
}
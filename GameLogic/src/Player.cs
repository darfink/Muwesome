using System;

namespace Muwesome.GameLogic {
  /// <summary>Represents a player in-game.</summary>
  public class Player : IDisposable {
    /// <summary>An event that is raised when the player is removed from the game.</summary>
    public event EventHandler Disposed;

    /// <inheritdoc />
    public void Dispose() {
      Disposed?.Invoke(this, EventArgs.Empty);
    }
  }
}
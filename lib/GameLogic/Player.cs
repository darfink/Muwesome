using System;
using System.Collections.Generic;
using System.Linq;
using Muwesome.GameLogic.Actions;

namespace Muwesome.GameLogic {
  /// <summary>Represents a player in-game.</summary>
  public class Player : IIdentifiable, IDisposable {
    private readonly Dictionary<Type, object> actions = new Dictionary<Type, object>();

    /// <summary>Initializes a new instance of the <see cref="Player"/> class.</summary>
    public Player() {
    }

    /// <summary>An event that is raised when the player is removed from the game.</summary>
    public event EventHandler Disposed;

    /// <inheritdoc />
    public ushort Id { get; set; }

    /// <summary>Gets an invokable action.</summary>
    public T Action<T>() where T : Delegate {
      actions.TryGetValue(typeof(T), out object result);
      return result as T;
    }

    /// <summary>Registers actions associated with the player.</summary>
    public void RegisterActions(params IPlayerActionProvider[] actions) =>
      this.RegisterActions(actions.AsEnumerable());

    /// <summary>Registers actions associated with the player.</summary>
    public void RegisterActions(IEnumerable<IPlayerActionProvider> actions) {
      foreach (var actionFactory in actions) {
        var action = actionFactory.CreateAction(this);
        this.actions[action.GetType()] = action;
      }
    }

    /// <inheritdoc />
    public void Dispose() {
      Disposed?.Invoke(this, EventArgs.Empty);
    }
  }
}
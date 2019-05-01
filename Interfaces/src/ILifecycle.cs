using System;
using System.Threading.Tasks;

namespace Muwesome.Interfaces {
  /// <summary>Represents an object that has a lifetime.</summary>
  public interface ILifecycle {
    /// <summary>An event that is raised when a lifecycle has started.</summary>
    event EventHandler<LifecycleEventArgs> AfterLifecycleStarted;

    /// <summary>An event that is raised whan a lifecycle has ended.</summary>
    event EventHandler<LifecycleEventArgs> AfterLifecycleEnded;

    /// <summary>Gets the lifecycle's task.</summary>
    Task ShutdownTask { get; }

    /// <summary>Starts the lifecycle.</summary>
    void Start();

    /// <summary>Stops the lifecycle.</summary>
    void Stop();
  }

  public class LifecycleEventArgs : EventArgs {
  }
}
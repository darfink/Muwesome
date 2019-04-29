using System;
using System.Threading.Tasks;

namespace Muwesome.ServerCommon {
  /// <summary>Represents an object that has a lifetime.</summary>
  public interface ILifecycle {
    /// <summary>Gets the lifecycle's task.</summary>
    Task Task { get; }

    /// <summary>Starts the lifecycle.</summary>
    void Start();

    /// <summary>Stops the lifecycle.</summary>
    void Stop();
  }
}
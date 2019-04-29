using System;
using System.Threading.Tasks;

namespace Muwesome.ServerCommon {
  public interface ILifecycle {
    /// <summary>Gets the lifecycle's task.</summary>
    Task Task { get; }

    /// <summary>Starts the lifecycle.</summary>
    void Start();

    /// <summary>Stops the lifecycle.</summary>
    void Stop();
  }
}
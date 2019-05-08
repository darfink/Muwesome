using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Muwesome.Interfaces;

namespace Muwesome.ServerCommon {
  public class LifecycleController : ILifecycle, IDisposable {
    private readonly ILifecycle[] lifecycleInstances;
    private int isRunning;

    /// <summary>Initializes a new instance of the <see cref="LifecycleController"/> class.</summary>
    public LifecycleController(params ILifecycle[] lifecycleInstances) =>
      this.lifecycleInstances = lifecycleInstances;

    /// <inheritdoc />
    public event EventHandler<LifecycleEventArgs> AfterLifecycleStarted;

    /// <inheritdoc />
    public event EventHandler<LifecycleEventArgs> AfterLifecycleEnded;

    /// <inheritdoc />
    public virtual Task ShutdownTask => Task.WhenAll(this.lifecycleInstances.Select(instance => instance.ShutdownTask));

    /// <summary>Gets the controller's logger instance.</summary>
    protected ILog Logger => LogManager.GetLogger(this.GetType());

    /// <summary>Gets the controller's identifier.</summary>
    protected virtual string Identifier => this.GetType().Name;

    /// <inheritdoc />
    public virtual void Start() {
      this.Logger.Info($"Starting {this.Identifier}...");
      foreach (var instance in this.lifecycleInstances) {
        instance.Start();
      }

      this.isRunning = 1;
      this.AfterLifecycleStarted?.Invoke(this, new LifecycleEventArgs());
      this.Logger.Info($"{this.Identifier} successfully started");
    }

    /// <inheritdoc />
    public virtual void Stop() {
      if (Interlocked.Exchange(ref this.isRunning, 0) == 0) {
        return;
      }

      this.Logger.Info($"Stopping {this.Identifier}...");
      foreach (var instance in this.lifecycleInstances) {
        instance.Stop();
      }

      this.ShutdownTask.Wait();
      this.AfterLifecycleEnded?.Invoke(this, new LifecycleEventArgs());
      this.Logger.Info($"{this.Identifier} stopped");
    }

    /// <inheritdoc />
    public virtual void Dispose() {
      this.Stop();
      foreach (var instance in this.lifecycleInstances.OfType<IDisposable>()) {
        instance.Dispose();
      }
    }
  }
}
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using log4net;

namespace Muwesome.ServerCommon {
  public abstract class LifecycleServerBase : ILifecycle, IDisposable {
    private readonly ILifecycle[] lifecycleInstances;
    private readonly Stopwatch startTime;
    private bool isRunning;

    /// <summary>Initializes a new instance of the <see cref="LifecycleServerBase"/> class.</summary>
    public LifecycleServerBase(params ILifecycle[] lifecycleInstances) {
      this.lifecycleInstances = lifecycleInstances;
      this.startTime = new Stopwatch();
      this.startTime.Start();
    }

    /// <summary>Gets the server's uptime.</summary>
    public TimeSpan Uptime => this.startTime.Elapsed;

    /// <inheritdoc />
    public Task Task => Task.WhenAll(this.lifecycleInstances.Select(instance => instance.Task));

    /// <summary>Gets the server's logger instance.</summary>
    protected ILog Logger => LogManager.GetLogger(this.GetType());

    /// <inheritdoc />
    public virtual void Start() {
      this.Logger.Info($"Starting {this.GetType().Name}...");
      foreach (var instance in this.lifecycleInstances) {
        instance.Start();
      }

      this.isRunning = true;
      this.Logger.Info("Server successfully started");
    }

    /// <inheritdoc />
    public virtual void Stop() {
      this.Logger.Info($"Stopping {this.GetType().Name}...");
      foreach (var instance in this.lifecycleInstances) {
        instance.Stop();
      }

      this.isRunning = false;
      this.Logger.Info("Server stopped");
    }

    /// <inheritdoc />
    public virtual void Dispose() {
      if (this.isRunning) {
        this.Stop();
      }

      foreach (var instance in this.lifecycleInstances.OfType<IDisposable>()) {
        instance.Dispose();
      }
    }
  }
}
using System;

namespace Muwesome.ConnectServer.Utility {
  /// <summary>A scope guard as an alternative to try/finally.</summary>
  internal struct ScopeGuard<T> : IDisposable {
    private readonly Action<T> dispose;
    private readonly T context;

    /// <summary>Initializes a new instance of the <see cref="ScopeGuard{T}"/> struct.</summary>
    public ScopeGuard(T context, Action<T> dispose) {
      this.context = context;
      this.dispose = dispose;
    }

    /// <inheritdoc />
    public void Dispose() => this.dispose(this.context);
  }
}
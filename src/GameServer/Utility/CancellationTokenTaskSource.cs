using System;
using System.Threading;
using System.Threading.Tasks;

namespace Muwesome.GameServer.Utility {
  public sealed class CancellationTokenTaskSource<T> : IDisposable {
    private readonly IDisposable registration;

    public CancellationTokenTaskSource(CancellationToken cancellationToken) {
      if (cancellationToken.IsCancellationRequested) {
        this.Task = System.Threading.Tasks.Task.FromCanceled<T>(cancellationToken);
        return;
      }

      var tcs = new TaskCompletionSource<T>();
      this.registration = cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken), useSynchronizationContext: false);
      this.Task = tcs.Task;
    }

    public Task<T> Task { get; private set; }

    public void Dispose() => this.registration?.Dispose();
  }
}
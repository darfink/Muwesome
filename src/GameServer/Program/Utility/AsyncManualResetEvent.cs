using System.Threading;
using System.Threading.Tasks;

namespace Muwesome.GameServer.Program.Utility {
  internal sealed class AsyncManualResetEvent {
    private readonly TaskCompletionSource<object> tcs;
    private readonly object mutex;

    public AsyncManualResetEvent(bool set = false) {
      this.mutex = new object();
      this.tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
      if (set) {
        this.tcs.TrySetResult(null);
      }
    }

    public Task WaitAsync() {
      lock (this.mutex) {
        return this.tcs.Task;
      }
    }

    public Task WaitAsync(CancellationToken cancellationToken) {
      var waitTask = this.WaitAsync();
      if (waitTask.IsCompleted) {
        return waitTask;
      }

      return cancellationToken.IsCancellationRequested
        ? Task.FromCanceled(cancellationToken)
        : this.AwaitTask(waitTask, cancellationToken);
    }

    public void Set() {
      lock (this.mutex) {
        this.tcs.TrySetResult(null);
      }
    }

    private async Task AwaitTask(Task waitTask, CancellationToken cancellationToken) {
      using (var cancelTaskSource = new CancellationTokenTaskSource<object>(cancellationToken)) {
        await await Task.WhenAny(waitTask, cancelTaskSource.Task);
      }
    }
  }
}
using System;
using System.Reflection;
using System.Threading.Tasks;
using log4net;

namespace Muwesome.GameServer {
  /// <summary>Game server program.</summary>
  internal static class Program {
    /// <summary>The server entry point.</summary>
    public static void Main() {
      var repository = LogManager.GetRepository(Assembly.GetEntryAssembly());
      log4net.Config.BasicConfigurator.Configure(repository);

      using (var server = GameServerFactory.Create(new Configuration())) {
        server.Start();
        Task.WaitAny(server.ShutdownTask, InterruptSignal());
        server.Stop();
      }
    }

    private static Task InterruptSignal() {
      var cancellation = new TaskCompletionSource<bool>();
      Console.CancelKeyPress += (sender, eventArgs) => {
        eventArgs.Cancel = true;
        cancellation.SetResult(true);
      };
      return cancellation.Task;
    }
  }
}
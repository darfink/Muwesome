using System;
using System.Reflection;
using System.Threading.Tasks;
using log4net;

namespace Muwesome.ConnectServer {
  public static class Program {
    public static void Main(string[] args) {
      var repository = LogManager.GetRepository(Assembly.GetEntryAssembly());
      log4net.Config.BasicConfigurator.Configure(repository);

      using (var server = ConnectServerFactory.Create(new Configuration())) {
        server.Start();
        Task.WaitAny(server.Task, InterruptSignal());
        server.Stop();
      }
    }

    public static Task InterruptSignal() {
      var cancellation = new TaskCompletionSource<bool>();
      Console.CancelKeyPress += (sender, eventArgs) => {
        eventArgs.Cancel = true;
        cancellation.SetResult(true);
      };
      return cancellation.Task;
    }
  }
}
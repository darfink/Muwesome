using System;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using Muwesome.ConnectServer.Program.Services;

namespace Muwesome.ConnectServer.Program {
  internal static class Program {
    public static void Main() {
      var repository = LogManager.GetRepository(Assembly.GetEntryAssembly());
      log4net.Config.BasicConfigurator.Configure(repository);

      using (var server = CreateServer(new ProgramConfiguration())) {
        server.Start();
        Task.WaitAny(server.ShutdownTask, InterruptSignal());
        server.Stop();
      }
    }

    private static ConnectServer CreateServer(ProgramConfiguration config) {
      var connectServer = ConnectServerFactory.Create(config);
      var serviceController = ServiceControllerFactory.Create(config.RpcService, connectServer);
      connectServer.AddDependency(serviceController);
      return connectServer;
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
using System;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using Muwesome.LoginServer.Program.Services;
using Muwesome.Persistence.EntityFramework;

namespace Muwesome.LoginServer.Program {
  /// <summary>Login server program.</summary>
  internal static class Program {
    /// <summary>The server entry point.</summary>
    public static void Main() {
      var repository = LogManager.GetRepository(Assembly.GetEntryAssembly());
      log4net.Config.BasicConfigurator.Configure(repository);

      using (var server = CreateServer(new ProgramConfiguration())) {
        server.Start();
        Task.WaitAny(server.ShutdownTask, InterruptSignal());
        server.Stop();
      }
    }

    private static LoginServer CreateServer(ProgramConfiguration config) {
      var persistenceContextProvider = new PersistenceContextProvider(config.PersistenceConfiguration);
      var loginServer = LoginServerFactory.Create(config, persistenceContextProvider);

      var serviceController = ServiceControllerFactory.Create(config.RpcService, loginServer);
      loginServer.AddDependency(serviceController);
      return loginServer;
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
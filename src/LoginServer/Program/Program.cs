using System;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using Muwesome.LoginServer.Program.Services;
using Muwesome.Persistence;
using Muwesome.Persistence.NHibernate;

namespace Muwesome.LoginServer.Program {
  /// <summary>Login server program.</summary>
  internal static class Program {
    /// <summary>The server entry point.</summary>
    public static void Main() {
      var repository = LogManager.GetRepository(Assembly.GetEntryAssembly());
      log4net.Config.BasicConfigurator.Configure(repository);

      var config = new ProgramConfiguration();
      using (var persistenceContextProvider = new PersistenceContextProvider(config.PersistenceConfiguration))
      using (var server = CreateServer(config, persistenceContextProvider)) {
        server.Start();
        Task.WaitAny(server.ShutdownTask, InterruptSignal());
        server.Stop();
      }
    }

    private static LoginServer CreateServer(ProgramConfiguration config, IPersistenceContextProvider persistenceContextProvider) {
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
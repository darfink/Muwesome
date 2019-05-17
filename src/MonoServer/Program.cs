using System;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using Muwesome.Common;
using Muwesome.MonoServer.Utility;
using Muwesome.Persistence;
using Muwesome.Persistence.Initialization;
using Muwesome.Persistence.NHibernate;

namespace Muwesome.MonoServer {
  /// <summary>Login server program.</summary>
  internal static class Program {
    /// <summary>The server entry point.</summary>
    public static void Main() {
      var repository = LogManager.GetRepository(Assembly.GetEntryAssembly());
      log4net.Config.BasicConfigurator.Configure(repository);

      using (var persistenceContextProvider = new PersistenceContextProvider(PersistenceConfiguration.InMemory())) {
        var initializer = new PersistenceInitializer(persistenceContextProvider);
        initializer.CreateConfiguration();
        initializer.CreateTestData();

        using (var connectServer = CreateConnectServer())
        using (var loginServer = CreateLoginServer(persistenceContextProvider))
        using (var gameServer = CreateGameServer(connectServer))
        using (BeginServerStart(connectServer))
        using (BeginServerStart(loginServer))
        using (BeginServerStart(gameServer)) {
            Task.WaitAny(gameServer.ShutdownTask, connectServer.ShutdownTask, InterruptSignal());
        }
      }
    }

    private static LoginServer.LoginServer CreateLoginServer(IPersistenceContextProvider persistenceContextProvider) {
      var config = new LoginServer.Configuration();
      return LoginServer.LoginServerFactory.Create(config, persistenceContextProvider);
    }

    private static ConnectServer.ConnectServer CreateConnectServer() {
      var config = new ConnectServer.Configuration();
      return ConnectServer.ConnectServerFactory.Create(config);
    }

    private static GameServer.GameServer CreateGameServer(IGameServerRegistrar gameServerRegistrar) {
      var config = new GameServer.Configuration();
      return GameServer.GameServerFactory.Create(config, gameServerRegistrar);
    }

    private static IDisposable BeginServerStart(ILifecycle server) {
      server.Start();
      return new ScopeGuard<ILifecycle>(server, s => s.Stop());
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
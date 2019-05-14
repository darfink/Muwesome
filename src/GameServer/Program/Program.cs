using System;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using Muwesome.GameServer.Program.Proxies;
using Muwesome.Persistence.NHibernate;

namespace Muwesome.GameServer.Program {
  /// <summary>Game server program.</summary>
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

    private static GameServer CreateServer(ProgramConfiguration config) {
      var gameServerRegistrar = new GameServerRegistrarProxy(config.ConnectServer);
      var gameServer = GameServerFactory.Create(config, gameServerRegistrar);
      return gameServer;
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
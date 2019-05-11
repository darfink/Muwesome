using Muwesome.LoginServer.Program.Services;
using Muwesome.Persistence.EntityFramework;
using Muwesome.ServerCommon;

namespace Muwesome.LoginServer.Program {
  public class ProgramConfiguration : Configuration {
    /// <summary>Gets or sets the RPC service end point.</summary>
    public RpcEndPoint RpcService { get; set; } = new RpcEndPoint() { Host = "127.0.0.1", Port = 22337 };

    /// <summary>Gets or sets the persistence configuration.</summary>
    public PersistenceConfiguration PersistenceConfiguration { get; set; } = PersistenceConfiguration.InMemory();
  }
}
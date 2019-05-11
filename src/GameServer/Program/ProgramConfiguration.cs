using System;
using System.Text;
using Muwesome.Persistence.EntityFramework;
using Muwesome.ServerCommon;

namespace Muwesome.GameServer.Program {
  /// <summary>The game server configuration.</summary>
  public class ProgramConfiguration : Configuration {
    /// <summary>Gets or sets the connect server RPC end point.</summary>
    public RpcEndPoint ConnectServer { get; set; } = new RpcEndPoint() { Host = "127.0.0.1", Port = 22336 };

    /// <summary>Gets or sets the login server RPC end point.</summary>
    public RpcEndPoint LoginServer { get; set; } = new RpcEndPoint() { Host = "127.0.0.1", Port = 22337 };

    /// <summary>Gets or sets the persistence configuration.</summary>
    public PersistenceConfiguration PersistenceConfiguration { get; set; } = PersistenceConfiguration.InMemory();
  }
}
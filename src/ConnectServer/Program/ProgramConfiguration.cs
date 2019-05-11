using Muwesome.ServerCommon;

namespace Muwesome.ConnectServer.Program {
  public class ProgramConfiguration : Configuration {
    /// <summary>Gets or sets the RPC service end point.</summary>
    public RpcEndPoint RpcService { get; set; } = new RpcEndPoint() { Host = "127.0.0.1", Port = 22336 };
  }
}
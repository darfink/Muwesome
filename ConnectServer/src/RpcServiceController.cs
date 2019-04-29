using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using Muwesome.ConnectServer.Rpc;
using Muwesome.ConnectServer.Services;
using Muwesome.ServerCommon;

namespace Muwesome.ConnectServer {
  public class RpcServiceController : RpcServiceControllerBase {
    private readonly IGameServerController gameServerController;

    /// <summary>Initializes a new instance of the <see cref="RpcServiceController"/> class.</summary>
    public RpcServiceController(Configuration config, IGameServerController gameServerController)
        : base(new ServerPort(config.GrpcListenerHost, config.GrpcListenerPort, ServerCredentials.Insecure)) =>
      this.gameServerController = gameServerController;

    /// <inheritdoc />
    protected override IEnumerable<ServerServiceDefinition> OnRegisterServices(CancellationToken token) {
      yield return GameServerRegister.BindService(new GameServerRegisterService(this.gameServerController, token));
    }
  }
}
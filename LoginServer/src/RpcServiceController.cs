using System;
using System.Collections.Generic;
using System.Threading;
using Grpc.Core;
using log4net;
using Muwesome.LoginServer.Rpc;
using Muwesome.LoginServer.Services;
using Muwesome.ServerCommon;

namespace Muwesome.LoginServer {
  public class RpcServiceController : RpcServiceControllerBase {
    /// <summary>Initializes a new instance of the <see cref="RpcServiceController"/> class.</summary>
    public RpcServiceController(Configuration config)
        : base(new ServerPort(config.GrpcListenerHost, config.GrpcListenerPort, ServerCredentials.Insecure)) {
    }

    /// <inheritdoc />
    protected override IEnumerable<ServerServiceDefinition> OnRegisterServices(CancellationToken token) {
      yield return AccountAuth.BindService(new AccountAuthService());
    }
  }
}
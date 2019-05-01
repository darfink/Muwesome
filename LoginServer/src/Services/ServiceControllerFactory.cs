using Grpc.Core;
using Muwesome.Interfaces;
using Muwesome.LoginServer.Rpc;
using Muwesome.ServerCommon;

namespace Muwesome.LoginServer.Services {
  internal static class ServiceControllerFactory {
    public static ILifecycle Create(Configuration config, AccountController accountController) {
      var port = new ServerPort(config.GrpcListenerHost, config.GrpcListenerPort, ServerCredentials.Insecure);
      var controller = new RpcServiceController(port);
      controller.RegisterService(token => AccountAuth.BindService(new AccountAuthService(accountController)));
      return controller;
    }
  }
}
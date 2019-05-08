using Grpc.Core;
using Muwesome.Interfaces;
using Muwesome.Rpc.LoginServer;
using Muwesome.ServerCommon;

namespace Muwesome.LoginServer.Services {
  internal static class ServiceControllerFactory {
    public static ILifecycle Create(Configuration config, AccountController accountController) {
      var port = new ServerPort(config.GrpcServiceHost, config.GrpcServicePort, ServerCredentials.Insecure);
      var controller = new RpcServiceController(port);
      controller.RegisterService(token => AccountAuth.BindService(new AccountAuthService(accountController, token)));
      return controller;
    }
  }
}
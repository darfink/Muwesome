using Grpc.Core;
using Muwesome.Common;
using Muwesome.Rpc;
using Muwesome.Rpc.LoginServer;

namespace Muwesome.LoginServer.Program.Services {
  internal static class ServiceControllerFactory {
    public static ILifecycle Create(RpcEndPoint endPoint, IAccountLoginService accountLoginService) {
      var port = new ServerPort(endPoint.Host, endPoint.Port, ServerCredentials.Insecure);
      var controller = new RpcServiceController(port);
      controller.RegisterService(token => AccountAuthentication.BindService(new AccountAuthenticationService(accountLoginService, token)));
      return controller;
    }
  }
}
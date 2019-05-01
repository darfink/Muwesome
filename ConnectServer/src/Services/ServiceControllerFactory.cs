using Grpc.Core;
using Muwesome.ConnectServer.Rpc;
using Muwesome.Interfaces;
using Muwesome.ServerCommon;

namespace Muwesome.ConnectServer.Services {
  internal static class ServiceControllerFactory {
    public static ILifecycle Create(Configuration config, GameServerController gameServerController) {
      var port = new ServerPort(config.GrpcListenerHost, config.GrpcListenerPort, ServerCredentials.Insecure);
      var controller = new RpcServiceController(port);
      controller.RegisterService(token => GameServerRegistrar.BindService(new GameServerRegistrarService(gameServerController, token)));
      return controller;
    }
  }
}
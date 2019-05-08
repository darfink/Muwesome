using Grpc.Core;
using Muwesome.Interfaces;
using Muwesome.Rpc.ConnectServer;
using Muwesome.ServerCommon;

namespace Muwesome.ConnectServer.Services {
  internal static class ServiceControllerFactory {
    public static ILifecycle Create(Configuration config, GameServerController gameServerController) {
      var port = new ServerPort(config.GrpcServiceHost, config.GrpcServicePort, ServerCredentials.Insecure);
      var controller = new RpcServiceController(port);
      controller.RegisterService(token => GameServerRegistrar.BindService(new GameServerRegistrarService(gameServerController, token)));
      return controller;
    }
  }
}
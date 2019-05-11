using Grpc.Core;
using Muwesome.Interfaces;
using Muwesome.Rpc.ConnectServer;
using Muwesome.ServerCommon;

namespace Muwesome.ConnectServer.Program.Services {
  internal static class ServiceControllerFactory {
    public static ILifecycle Create(RpcEndPoint endPoint, IGameServerRegistrar gameServerRegistrar) {
      var port = new ServerPort(endPoint.Host, endPoint.Port, ServerCredentials.Insecure);
      var controller = new RpcServiceController(port);
      controller.RegisterService(token => GameServerRegistrar.BindService(new GameServerRegistrarService(gameServerRegistrar, token)));
      return controller;
    }
  }
}
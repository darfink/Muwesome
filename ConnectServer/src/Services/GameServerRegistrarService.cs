using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Muwesome.ConnectServer.Rpc;

namespace Muwesome.ConnectServer.Services {
  public class GameServerRegistrarService : GameServerRegistrar.GameServerRegistrarBase {
    private readonly IGameServerController gameServerController;
    private readonly CancellationToken cancellationToken;

    /// <summary>Initializes a new instance of the <see cref="GameServerRegistrarService"/> class.</summary>
    public GameServerRegistrarService(IGameServerController gameServerController, CancellationToken cancellationToken) {
      this.gameServerController = gameServerController;
      this.cancellationToken = cancellationToken;
    }

    /// <summary>Processes an incoming game server session.</summary>
    public override async Task<GameServerRegisterResponse> RegisterGameServer(
        IAsyncStreamReader<GameServerParams> requestStream,
        ServerCallContext context) {
      var server = await this.GameServerRegisterAsync(requestStream);

      try {
        await this.GameServerUpdatesAsync(requestStream, server);
      } finally {
        this.gameServerController.DeregisterServer(server);
      }

      return new GameServerRegisterResponse();
    }

    private async Task<GameServerEntry> GameServerRegisterAsync(IAsyncStreamReader<GameServerParams> requestStream) {
      if (!await requestStream.MoveNext(this.cancellationToken)) {
        throw new RpcException(Status.DefaultCancelled);
      }

      var register = requestStream.Current.Register;
      if (register is null) {
        throw new RpcException(new Status(StatusCode.InvalidArgument, "The server is not registered; expected a 'Register' message"));
      }

      GameServerEntry server = null;
      try {
        server = checked(new GameServerEntry((byte)register.Code, register.Host, (ushort)register.Port, register.Status.ClientCount, register.Status.ClientCapacity));
      } catch (OverflowException) {
        throw new RpcException(new Status(StatusCode.InvalidArgument, "The server fields are out of range"));
      }

      try {
        this.gameServerController.RegisterServer(server);
      } catch (ArgumentException) {
        throw new RpcException(new Status(StatusCode.InvalidArgument, "The server code is already registered"));
      }

      return server;
    }

    private async Task GameServerUpdatesAsync(IAsyncStreamReader<GameServerParams> requestStream, GameServerEntry server) {
      while (await requestStream.MoveNext(this.cancellationToken)) {
        var status = requestStream.Current.Status;
        if (status is null) {
          throw new RpcException(new Status(StatusCode.InvalidArgument, "The server is already registered; expected a 'Status' message"));
        }

        server.ClientCount = status.ClientCount;
        server.ClientCapacity = status.ClientCapacity;
      }
    }
  }
}
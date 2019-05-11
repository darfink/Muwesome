using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Muwesome.Interfaces;
using Muwesome.Rpc.ConnectServer;

namespace Muwesome.ConnectServer.Program.Services {
  internal class GameServerRegistrarService : GameServerRegistrar.GameServerRegistrarBase {
    private readonly IGameServerRegistrar gameServerRegistrar;
    private readonly CancellationToken cancellationToken;

    /// <summary>Initializes a new instance of the <see cref="GameServerRegistrarService"/> class.</summary>
    public GameServerRegistrarService(IGameServerRegistrar gameServerRegistrar, CancellationToken cancellationToken) {
      this.gameServerRegistrar = gameServerRegistrar;
      this.cancellationToken = cancellationToken;
    }

    /// <summary>Processes an incoming game server session.</summary>
    public override async Task<GameServerRegisterResponse> RegisterGameServer(
        IAsyncStreamReader<GameServerRequest> requestStream,
        ServerCallContext context) {
      var server = await this.GameServerRegisterAsync(requestStream);

      try {
        await this.GameServerUpdatesAsync(requestStream, server);
      } finally {
        await this.gameServerRegistrar.DeregisterGameServerAsync(server.Code);
      }

      return new GameServerRegisterResponse();
    }

    private async Task<GameServerInfo> GameServerRegisterAsync(IAsyncStreamReader<GameServerRequest> requestStream) {
      if (!await requestStream.MoveNext(this.cancellationToken)) {
        throw new RpcException(Status.DefaultCancelled);
      }

      var register = requestStream.Current.Register;
      if (register is null) {
        throw new RpcException(new Status(StatusCode.InvalidArgument, "The server is not registered; expected a 'Register' message"));
      }

      GameServerInfo server = null;
      try {
        server = checked(new GameServerInfo((byte)register.Code, register.Host, (ushort)register.Port, register.Status.ClientCount, register.Status.ClientCapacity));
      } catch (OverflowException) {
        throw new RpcException(new Status(StatusCode.InvalidArgument, "The server fields are out of range"));
      }

      try {
        await this.gameServerRegistrar.RegisterGameServerAsync(server);
      } catch (ArgumentException) {
        throw new RpcException(new Status(StatusCode.InvalidArgument, "The server code is already registered"));
      }

      return server;
    }

    private async Task GameServerUpdatesAsync(IAsyncStreamReader<GameServerRequest> requestStream, GameServerInfo server) {
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
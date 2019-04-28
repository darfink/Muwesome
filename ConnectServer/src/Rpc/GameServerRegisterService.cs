using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace Muwesome.ConnectServer.Rpc {
  public class GameServerRegisterService : GameServerRegister.GameServerRegisterBase {
    private readonly IGameServerController _gameServerController;
    private readonly CancellationToken _cancellationToken;

    /// <summary>Creates a new <see cref="GameServerRegisterService" />.</summary>
    public GameServerRegisterService(IGameServerController gameServerController, CancellationToken cancellationToken) {
      _gameServerController = gameServerController;
      _cancellationToken = cancellationToken;
    }

    /// <summary>Processes an incoming game server session.</summary>
    public override async Task<GameServerRegisterResponse> RegisterGameServer(
      IAsyncStreamReader<GameServerParams> requestStream,
      ServerCallContext context
    ) {
      var server = await GameServerRegisterAsync(requestStream);

      try {
        await GameServerUpdatesAsync(requestStream, server);
      } finally {
        _gameServerController.DeregisterServer(server);
      }

      return new GameServerRegisterResponse();
    }

    private async Task<GameServer> GameServerRegisterAsync(IAsyncStreamReader<GameServerParams> requestStream) {
      if (!await requestStream.MoveNext(_cancellationToken)) {
        throw new RpcException(Status.DefaultCancelled);
      }

      var register = requestStream.Current.Register;
      if (register is null) {
        throw new RpcException(new Status(StatusCode.InvalidArgument, "The server is not registered; expected a 'Register' message"));
      }

      GameServer server = null;
      try {
        server = checked(new GameServer((byte)register.Id, register.Host, (ushort)register.Port, register.Status.Clients, register.Status.Capacity));
      } catch (OverflowException) {
        throw new RpcException(new Status(StatusCode.InvalidArgument, "The server fields are out of range"));
      }

      try {
        _gameServerController.RegisterServer(server);
      } catch (ArgumentException) {
        throw new RpcException(new Status(StatusCode.InvalidArgument, "The server ID is already registered"));
      }

      return server;
    }

    private async Task GameServerUpdatesAsync(IAsyncStreamReader<GameServerParams> requestStream, GameServer server) {
      while (await requestStream.MoveNext(_cancellationToken)) {
        var status = requestStream.Current.Status;
        if (status is null) {
          throw new RpcException(new Status(StatusCode.InvalidArgument, "The server is already registered; expected a 'Status' message"));
        }

        server.ClientCount = status.Clients;
        server.ClientCapacity = status.Capacity;
      }
    }
  }
}
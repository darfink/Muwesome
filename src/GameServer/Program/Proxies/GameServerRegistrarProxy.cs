using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using Muwesome.Interfaces;
using Muwesome.Network.Tcp;
using Muwesome.Rpc.ConnectServer;
using Muwesome.ServerCommon;
using Muwesome.ServerCommon.Utility;

namespace Muwesome.GameServer.Program.Proxies {
  /// <summary>A game server registrar proxy which forwards all requests to an RPC service.</summary>
  internal class GameServerRegistrarProxy : IGameServerRegistrar, IDisposable {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(GameServerRegistrarProxy));
    private readonly object channelLock = new object();
    private readonly ConcurrentDictionary<ushort, (Task, CancellationTokenSource)> gameServerSessions = new ConcurrentDictionary<ushort, (Task, CancellationTokenSource)>();
    private readonly RpcEndPoint endPoint;
    private CancellationTokenSource cancellationTokenSource;
    private Channel channel;

    /// <summary>Initializes a new instance of the <see cref="GameServerRegistrarProxy"/> class.</summary>
    public GameServerRegistrarProxy(RpcEndPoint endPoint) => this.endPoint = endPoint;

    /// <inheritdoc />
    // TODO: Only counting from this end is insufficient
    public int GameServersRegistered => throw new NotImplementedException();

    /// <summary>Gets the shutdown task over all registrations.</summary>
    public Task ShutdownTask => Task.WhenAll(this.gameServerSessions.Select(t => t.Value.Item1));

    /// <summary>Gets the connection channel lazily.</summary>
    private Task<Channel> LazyChannel {
      get {
        lock (this.channelLock) {
          if (this.channel == null || this.channel.State == ChannelState.Idle) {
            return this.ConnectWithChannel();
          } else {
            return Task.FromResult(this.channel);
          }
        }
      }
    }

    /// <inheritdoc />
    public async Task RegisterGameServerAsync(GameServerInfo server) {
      Logger.Info($"Registering game server ({server.Code}) at {this.endPoint}...");
      var channel = await this.LazyChannel;

      var client = new GameServerRegistrar.GameServerRegistrarClient(channel);
      var stream = client.RegisterGameServer(cancellationToken: this.cancellationTokenSource.Token);
      try {
        await stream.RequestStream.WriteAsync(new GameServerRequest {
          Register = new GameServerRequest.Types.Register {
            Code = server.Code,
            Host = server.Host,
            Port = server.Port,
            Status = this.ConvertToStatusUpdate(server),
          },
        });
      } catch (Exception) {
        stream.Dispose();
        throw;
      }

      // Any player count changes must also be tracked and reported
      var (sessionTask, sessionCancellationSource) = this.SetupGameServerChangeTracking(server, stream);

      if (!this.gameServerSessions.TryAdd(server.Code, (sessionTask, sessionCancellationSource))) {
        sessionCancellationSource.Cancel();
        throw new ArgumentException($"Game server ({server.Code}) is already registered", nameof(server));
      }

      Logger.Info($"Game server ({server.Code}) registered");
    }

    /// <inheritdoc />
    public async Task DeregisterGameServerAsync(ushort serverCode) {
      if (this.gameServerSessions.TryRemove(serverCode, out (Task Task, CancellationTokenSource CancellationSource) session)) {
        session.CancellationSource.Cancel();
        await session.Task;
      }
    }

    /// <inheritdoc />
    public void Dispose() {
      var cancelSource = Interlocked.Exchange(ref this.cancellationTokenSource, null);

      if (cancelSource != null) {
        cancelSource.Cancel();
        cancelSource.Dispose();
        this.ShutdownTask.Wait();

        if (this.channel.State == ChannelState.Connecting || this.channel.State == ChannelState.Ready) {
          try {
            this.channel.ShutdownAsync().Wait();
          } catch (Exception ex) when (ex.FindExceptionByType<TaskCanceledException>() != null) {
          }
        }
      }
    }

    private (Task, CancellationTokenSource) SetupGameServerChangeTracking(
        GameServerInfo server,
        AsyncClientStreamingCall<GameServerRequest, GameServerRegisterResponse> stream) {
      server.PropertyChanged += OnGameServerStatusChanged;
      var sessionCancellationSource = new CancellationTokenSource();
      var sessionTask = Task
        .Run(async () => await stream.ResponseAsync, sessionCancellationSource.Token)
        .ContinueWith(OnGameServerSessionEnded);

      return (sessionTask, sessionCancellationSource);

      async void OnGameServerStatusChanged(object o, PropertyChangedEventArgs ev) {
        await stream.RequestStream.WriteAsync(new GameServerRequest { Status = this.ConvertToStatusUpdate(server) });
      }

      async Task OnGameServerSessionEnded(Task task) {
        server.PropertyChanged -= OnGameServerStatusChanged;
        this.gameServerSessions.TryRemove(server.Code, out _);
        stream.Dispose();

        bool gameServerDeregisteredPrematurely =
          !this.cancellationTokenSource.IsCancellationRequested &&
          task.Exception.FindExceptionByType<RpcException>().StatusCode == StatusCode.Cancelled;

        if (gameServerDeregisteredPrematurely) {
          Logger.Info($"Game server deregistered prematurely; attempting to reregister");
          await Task.Delay(TimeSpan.FromSeconds(1));
          await this.RegisterGameServerAsync(server);
        } else if (task.Exception != null) {
          Logger.Error($"Game server ({server.Code}) deregistered due to an error", task.Exception);
        } else {
          Logger.Info($"Game server ({server.Code}) deregistered");
        }
      }
    }

    private GameServerRequest.Types.StatusUpdate ConvertToStatusUpdate(GameServerInfo server) {
      return checked(new GameServerRequest.Types.StatusUpdate {
        ClientCount = server.ClientCount,
        ClientCapacity = server.ClientCapacity,
      });
    }

    private async Task<Channel> ConnectWithChannel() {
      this.cancellationTokenSource = new CancellationTokenSource();
      this.channel = new Channel(this.endPoint.Host, this.endPoint.Port, ChannelCredentials.Insecure);

      using (this.cancellationTokenSource.Token.Register(() => this.channel.ShutdownAsync())) {
        await this.channel.ConnectAsync();
      }

      return this.channel;
    }
  }
}
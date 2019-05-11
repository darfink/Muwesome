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

namespace Muwesome.GameServer.Program.Clients {
  /// <summary>A connect server registerer.</summary>
  internal class GameServerRegisterer : IGameServerRegistrar, IDisposable {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(GameServerRegisterer));
    private readonly ConcurrentDictionary<ushort, (CancellationTokenSource, Task)> gameServerSessions = new ConcurrentDictionary<ushort, (CancellationTokenSource, Task)>();
    private readonly Lazy<Task<Channel>> channelConnection;
    private readonly RpcEndPoint endPoint;
    private CancellationTokenSource cancellationTokenSource;

    /// <summary>Initializes a new instance of the <see cref="GameServerRegisterer"/> class.</summary>
    public GameServerRegisterer(RpcEndPoint endPoint) {
      this.channelConnection = new Lazy<Task<Channel>>(() => this.Connect());
      this.endPoint = endPoint;
    }

    /// <inheritdoc />
    // TODO: Only counting from this end is insufficient
    public int GameServersRegistered => throw new NotImplementedException();

    /// <summary>Gets the shutdown task over all registrations.</summary>
    public Task ShutdownTask => Task.WhenAll(this.gameServerSessions.Select(t => t.Value.Item2));

    /// <summary>Gets the connection channel lazily.</summary>
    private Task<Channel> Channel => this.channelConnection.Value;

    /// <inheritdoc />
    public async Task RegisterGameServerAsync(GameServerInfo server) {
      Logger.Info($"Registering game server ({server.Code}) at {this.endPoint}...");
      var channel = await this.Channel;

      var client = new GameServerRegistrar.GameServerRegistrarClient(channel);
      var registration = client.RegisterGameServer(cancellationToken: this.cancellationTokenSource.Token);
      try {
        await registration.RequestStream.WriteAsync(new GameServerRequest {
          Register = new GameServerRequest.Types.Register {
            Code = server.Code,
            Host = server.Host,
            Port = server.Port,
            Status = this.ConvertToStatusUpdate(server),
          },
        });
      } catch (Exception) {
        registration.Dispose();
        throw;
      }

      server.PropertyChanged += updateGameServerStatus;
      var registrationCancellationSource = new CancellationTokenSource();
      var registrationTask = Task
        .Run(async () => await registration.ResponseAsync, registrationCancellationSource.Token)
        .ContinueWith(task => {
          server.PropertyChanged -= updateGameServerStatus;
          this.gameServerSessions.TryRemove(server.Code, out _);
          registration.Dispose();
          Logger.Info($"Game server ({server.Code}) deregistered");
        });

      if (!this.gameServerSessions.TryAdd(server.Code, (registrationCancellationSource, registrationTask))) {
        registrationCancellationSource.Cancel();
        throw new ArgumentException($"Game server ({server.Code}) is already registered", nameof(server));
      }

      Logger.Info($"Game server ({server.Code}) registered");

      async void updateGameServerStatus(object o, PropertyChangedEventArgs ev) {
        await registration.RequestStream.WriteAsync(new GameServerRequest { Status = this.ConvertToStatusUpdate(server) });
      }
    }

    /// <inheritdoc />
    public async Task DeregisterGameServerAsync(ushort serverCode) {
      if (this.gameServerSessions.TryRemove(serverCode, out (CancellationTokenSource CancellationSource, Task Task) pair)) {
        pair.CancellationSource.Cancel();
        await pair.Task;
      } else {
        Logger.Warn($"Tried to deregister unregistered game server ({serverCode})");
      }
    }

    /// <inheritdoc />
    public void Dispose() => this.CancelRegistration();

    private void CancelRegistration() {
      var cancelSource = Interlocked.Exchange(ref this.cancellationTokenSource, null);

      if (cancelSource != null) {
        cancelSource.Cancel();
        cancelSource.Dispose();
      }
    }

    private GameServerRequest.Types.StatusUpdate ConvertToStatusUpdate(GameServerInfo server) {
      return checked(new GameServerRequest.Types.StatusUpdate {
        ClientCount = server.ClientCount,
        ClientCapacity = server.ClientCapacity,
      });
    }

    private async Task<Channel> Connect() {
      this.cancellationTokenSource = new CancellationTokenSource();
      var channel = new Channel(this.endPoint.Host, this.endPoint.Port, ChannelCredentials.Insecure);

      using (this.cancellationTokenSource.Token.Register(() => channel.ShutdownAsync())) {
        await channel.ConnectAsync();
      }

      return channel;
    }
  }
}
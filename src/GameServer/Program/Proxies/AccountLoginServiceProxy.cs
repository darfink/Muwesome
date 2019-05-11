using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using Muwesome.Common;
using Muwesome.Common.Utility;
using Muwesome.DomainModel.Entities;
using Muwesome.GameServer.Program.Utility;
using Muwesome.Persistence;
using Muwesome.Rpc;
using Muwesome.Rpc.LoginServer;

namespace Muwesome.GameServer.Program.Proxies {
  /// <summary>A account login service proxy which forwards all requests to an RPC service.</summary>
  internal class AccountLoginServiceProxy : IAccountLoginService, ILifecycle, IDisposable {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(AccountLoginServiceProxy));
    private readonly object enqueueLock = new object();
    private readonly ConcurrentQueue<TaskCompletionSource<LoginError?>> pendingLoginTasks = new ConcurrentQueue<TaskCompletionSource<LoginError?>>();
    private readonly ConcurrentQueue<TaskCompletionSource<bool>> pendingLogoutTasks = new ConcurrentQueue<TaskCompletionSource<bool>>();
    private readonly ConcurrentQueue<AuthRequest> pendingAuthRequests = new ConcurrentQueue<AuthRequest>();
    private readonly AsyncManualResetEvent incomingAuthRequestSignal = new AsyncManualResetEvent();
    private readonly RpcEndPoint endPoint;
    private CancellationTokenSource cancellationTokenSource;

    /// <summary>Initializes a new instance of the <see cref="AccountLoginServiceProxy"/> class.</summary>
    public AccountLoginServiceProxy(RpcEndPoint endPoint) => this.endPoint = endPoint;

    /// <inheritdoc />
    public event EventHandler<LifecycleEventArgs> LifecycleStarted;

    /// <inheritdoc />
    public event EventHandler<LifecycleEventArgs> LifecycleEnded;

    /// <inheritdoc />
    public Task ShutdownTask { get; private set; } = Task.CompletedTask;

    /// <inheritdoc />
    // TODO: Only counting from this end is insufficient
    public int AccountsLoggedIn => throw new NotImplementedException();

    /// <inheritdoc />
    public void Start() {
      this.cancellationTokenSource = new CancellationTokenSource();
      Channel channel = new Channel(this.endPoint.Host, this.endPoint.Port, ChannelCredentials.Insecure);
      this.ShutdownTask = this.BeginAuthSession(channel, this.cancellationTokenSource.Token)
        .ContinueWith(t => this.OnSessionComplete(t.Exception));
      this.LifecycleStarted?.Invoke(this, new LifecycleEventArgs());
    }

    /// <inheritdoc />
    public void Stop() {
      var cancelSource = Interlocked.Exchange(ref this.cancellationTokenSource, null);

      if (cancelSource != null) {
        cancelSource.Cancel();
        cancelSource.Dispose();
      }
    }

    /// <inheritdoc />
    public void Dispose() => this.Stop();

    /// <inheritdoc />
    public Task<LoginError?> TryLoginAsync(string username, string password) {
      var taskCompletionSource = new TaskCompletionSource<LoginError?>();
      var authRequest = new AuthRequest {
        Login = new AuthRequest.Types.Login {
          Username = username,
          Password = password,
        },
      };

      lock (this.enqueueLock) {
        this.pendingLoginTasks.Enqueue(taskCompletionSource);
        this.pendingAuthRequests.Enqueue(authRequest);
      }

      this.incomingAuthRequestSignal.Set();
      return taskCompletionSource.Task;
    }

    /// <inheritdoc />
    public Task<bool> TryLogoutAsync(string username) {
      var taskCompletionSource = new TaskCompletionSource<bool>();
      var authRequest = new AuthRequest {
        Logout = new AuthRequest.Types.Logout { Username = username },
      };

      lock (this.enqueueLock) {
        this.pendingLogoutTasks.Enqueue(taskCompletionSource);
        this.pendingAuthRequests.Enqueue(authRequest);
      }

      this.incomingAuthRequestSignal.Set();
      return taskCompletionSource.Task;
    }

    private async Task BeginAuthSession(Channel channel, CancellationToken cancellationToken) {
      Logger.Info($"Connecting to login server at {channel.Target}...");
      using (cancellationToken.Register(() => channel.ShutdownAsync())) {
        await channel.ConnectAsync();
      }

      Logger.Info($"Connected to login server");
      var client = new AccountAuthentication.AccountAuthenticationClient(channel);
      var stream = client.RegisterAuthSession(cancellationToken: cancellationToken);

      while (!cancellationToken.IsCancellationRequested) {
        await this.incomingAuthRequestSignal.WaitAsync(cancellationToken);

        while (this.pendingAuthRequests.TryDequeue(out AuthRequest request)) {
          // Send each auth request sequentially due to stream limitations
          await stream.RequestStream.WriteAsync(request);

          switch (request.TypeCase) {
          case AuthRequest.TypeOneofCase.Login:
            await stream.ResponseStream.MoveNext(cancellationToken);
            this.ProcessLoginResponse(stream.ResponseStream.Current);
            break;
          case AuthRequest.TypeOneofCase.Logout:
            await stream.ResponseStream.MoveNext(cancellationToken);
            this.ProcessLogoutResponse(stream.ResponseStream.Current);
            break;
          default:
            throw new InvalidEnumArgumentException(nameof(request), (int)request.TypeCase, typeof(AuthRequest.TypeOneofCase));
          }
        }
      }
    }

    private void ProcessLoginResponse(AuthResponse response) {
      this.pendingLoginTasks.TryDequeue(out TaskCompletionSource<LoginError?> loginTask);
      loginTask.SetResult(this.ConvertLoginResult(response.Result));
    }

    private void ProcessLogoutResponse(AuthResponse response) {
      this.pendingLogoutTasks.TryDequeue(out TaskCompletionSource<bool> logoutTask);
      bool successfullyLoggedOut = response.Result == AuthResponse.Types.LoginResult.Success;
      logoutTask.SetResult(successfullyLoggedOut);
    }

    private void OnSessionComplete(Exception ex) {
      if (ex != null && ex.FindExceptionByType<RpcException>()?.StatusCode != StatusCode.Cancelled) {
        Logger.Error("An unexpected error occurred during the login server session", ex);
      }

      this.LifecycleEnded?.Invoke(this, new LifecycleEventArgs());
    }

    private LoginError? ConvertLoginResult(AuthResponse.Types.LoginResult loginResult) {
      switch (loginResult) {
        case AuthResponse.Types.LoginResult.Success: return null;
        case AuthResponse.Types.LoginResult.InvalidAccount: return LoginError.InvalidAccount;
        case AuthResponse.Types.LoginResult.InvalidPassword: return LoginError.InvalidPassword;
        case AuthResponse.Types.LoginResult.AccountIsBlocked: return LoginError.AccountIsBlocked;
        case AuthResponse.Types.LoginResult.AccountIsLockedOut: return LoginError.AccountIsLockedOut;
        case AuthResponse.Types.LoginResult.AccountIsAlreadyConnected: return LoginError.AccountIsAlreadyConnected;
        default: throw new InvalidEnumArgumentException(nameof(loginResult), (int)loginResult, loginResult.GetType());
      }
    }
  }
}
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using Muwesome.DomainModel.Entities;
using Muwesome.GameLogic;
using Muwesome.GameServer.Utility;
using Muwesome.Interfaces;
using Muwesome.Persistence;
using Muwesome.Rpc.LoginServer;
using OneOf;

namespace Muwesome.GameServer {
  internal class AccountAuthenticator : ILifecycle, IDisposable {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(AccountAuthenticator));
    private readonly ConcurrentQueue<TaskCompletionSource<AccountOrLoginError>> pendingLoginTasks = new ConcurrentQueue<TaskCompletionSource<AccountOrLoginError>>();
    private readonly ConcurrentQueue<AuthRequest> pendingAuthRequests = new ConcurrentQueue<AuthRequest>();
    private readonly AsyncManualResetEvent incomingAuthRequestSignal = new AsyncManualResetEvent();
    private readonly IPersistenceContextProvider persistenceContextProvider;
    private readonly Configuration config;
    private CancellationTokenSource cancellationTokenSource;

    /// <summary>Initializes a new instance of the <see cref="AccountAuthenticator"/> class.</summary>
    public AccountAuthenticator(Configuration config, IPersistenceContextProvider persistenceContextProvider) {
      this.persistenceContextProvider = persistenceContextProvider;
      this.config = config;
    }

    /// <inheritdoc />
    public event EventHandler<LifecycleEventArgs> LifecycleStarted;

    /// <inheritdoc />
    public event EventHandler<LifecycleEventArgs> LifecycleEnded;

    /// <inheritdoc />
    public Task ShutdownTask { get; private set; } = Task.CompletedTask;

    /// <inheritdoc />
    public void Start() {
      this.cancellationTokenSource = new CancellationTokenSource();
      Channel channel = new Channel(this.config.LoginServerGrpcHost, this.config.LoginServerGrpcPort, ChannelCredentials.Insecure);
      this.ShutdownTask = this.BeginAuthSession(channel, this.cancellationTokenSource.Token)
        .ContinueWith(t => this.OnSessionComplete(t.Exception));
      this.LifecycleStarted?.Invoke(this, new LifecycleEventArgs());
    }

    /// <inheritdoc />
    public void Stop() => this.CancelSession();

    /// <inheritdoc />
    public void Dispose() => this.Stop();

    public Task<AccountOrLoginError> Login(string username, string password) {
      var taskCompletionSource = new TaskCompletionSource<AccountOrLoginError>();
      var authRequest = new AuthRequest {
        Login = new AuthRequest.Types.Login {
          Username = username,
          Password = password,
        },
      };

      this.pendingLoginTasks.Enqueue(taskCompletionSource);
      this.pendingAuthRequests.Enqueue(authRequest);
      this.incomingAuthRequestSignal.Set();

      return taskCompletionSource.Task;
    }

    private async Task BeginAuthSession(Channel channel, CancellationToken cancellationToken) {
      Logger.Info($"Connecting to login server at {channel.Target}...");
      using (cancellationToken.Register(() => channel.ShutdownAsync())) {
        await channel.ConnectAsync();
      }

      var client = new AccountAuth.AccountAuthClient(channel);
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
          }
        }
      }
    }

    private void ProcessLoginResponse(AuthResponse login) {
      this.pendingLoginTasks.TryDequeue(out TaskCompletionSource<AccountOrLoginError> loginTask);

      if (login.Result != AuthResponse.Types.LoginResult.Success) {
        loginTask.SetResult((AccountOrLoginError)this.ConvertLoginResult(login.Result));
        return;
      }

      var accountId = new Guid(login.AccountId.ToByteArray());
      using (var context = this.persistenceContextProvider.CreateContext()) {
        // TODO: Perform this asynchronously
        var account = context.GetById<Account>(accountId);

        if (account == null) {
          Logger.Error($"Received a non-existing account ID from login server; {accountId}");
          loginTask.SetResult((AccountOrLoginError)GameLogic.Actions.LoginResult.InternalError);
        } else {
          loginTask.SetResult((AccountOrLoginError)account);
        }
      }
    }

    private void OnSessionComplete(Exception ex) {
      if (ex != null && ex.GetExceptionByType<RpcException>()?.StatusCode != StatusCode.Cancelled) {
        Logger.Error("An unexpected error occurred during the login server session", ex);
      }

      this.LifecycleEnded?.Invoke(this, new LifecycleEventArgs());
    }

    private void CancelSession() {
      var cancelSource = Interlocked.Exchange(ref this.cancellationTokenSource, null);

      if (cancelSource != null) {
        cancelSource.Cancel();
        cancelSource.Dispose();
      }
    }

    private GameLogic.Actions.LoginResult ConvertLoginResult(AuthResponse.Types.LoginResult loginResult) {
      switch (loginResult) {
      case AuthResponse.Types.LoginResult.Success: return Muwesome.GameLogic.Actions.LoginResult.Success;
      case AuthResponse.Types.LoginResult.InvalidPassword: return Muwesome.GameLogic.Actions.LoginResult.InvalidPassword;
      case AuthResponse.Types.LoginResult.InvalidAccount: return Muwesome.GameLogic.Actions.LoginResult.InvalidAccount;
      case AuthResponse.Types.LoginResult.AccountIsBlocked: return Muwesome.GameLogic.Actions.LoginResult.AccountIsBlocked;
      case AuthResponse.Types.LoginResult.AccountIsLockedOut: return Muwesome.GameLogic.Actions.LoginResult.TooManyFailedLoginAttempts;
      case AuthResponse.Types.LoginResult.AccountIsAlreadyConnected: return Muwesome.GameLogic.Actions.LoginResult.AccountIsAlreadyConnected;
      default: throw new InvalidEnumArgumentException(nameof(loginResult), (int)loginResult, loginResult.GetType());
      }
    }
  }
}
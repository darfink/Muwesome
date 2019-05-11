using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using Muwesome.Common;
using Muwesome.DomainModel.Entities;
using Muwesome.Persistence;
using Muwesome.Rpc.LoginServer;

namespace Muwesome.LoginServer.Program.Services {
  internal class AccountAuthenticationService : AccountAuthentication.AccountAuthenticationBase {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(AccountAuthenticationService));
    private readonly IAccountLoginService accountLoginService;
    private readonly CancellationToken cancellationToken;

    /// <summary>Initializes a new instance of the <see cref="AccountAuthenticationService"/> class.</summary>
    public AccountAuthenticationService(
        IAccountLoginService accountLoginService,
        CancellationToken cancellationToken) {
      this.accountLoginService = accountLoginService;
      this.cancellationToken = cancellationToken;
    }

    public override async Task RegisterAuthSession(
        IAsyncStreamReader<AuthRequest> requestStream,
        IServerStreamWriter<AuthResponse> responseStream,
        ServerCallContext context) {
      var activeAccounts = new HashSet<string>();
      try {
        while (await requestStream.MoveNext(this.cancellationToken)) {
          if (requestStream.Current.Login != null) {
            var response = await this.Login(requestStream.Current.Login, activeAccounts);
            await responseStream.WriteAsync(response);
          } else {
            await this.Logout(requestStream.Current.Logout, activeAccounts);
          }
        }
      } catch (Exception ex) when (!(ex is RpcException)) {
        Logger.Error("An unexpected error occured whilst processing auth requests", ex);
      } finally {
        await Task.WhenAll(activeAccounts.Select(username => this.accountLoginService.TryLogoutAsync(username)));
      }
    }

    private async Task<AuthResponse> Login(AuthRequest.Types.Login login, ISet<string> activeAccounts) {
      Logger.Debug($"Login attempt for {login.Username}...");
      var error = await this.accountLoginService.TryLoginAsync(login.Username, login.Password);

      if (error == null) {
        Logger.Debug($"Login successful for {login.Username}...");
        activeAccounts.Add(login.Username);
      } else {
        Logger.Info($"Login failed for {login.Username}; {error}");
      }

      return new AuthResponse { Result = this.ConvertToRpcLoginError(error) };
    }

    private async Task<AuthResponse> Logout(AuthRequest.Types.Logout logout, ISet<string> activeAccounts) {
      if (!await this.accountLoginService.TryLogoutAsync(logout.Username)) {
        return new AuthResponse { Result = AuthResponse.Types.LoginResult.InvalidAccount };
      }

      activeAccounts.Remove(logout.Username);
      return new AuthResponse { Result = AuthResponse.Types.LoginResult.Success };
    }

    private AuthResponse.Types.LoginResult ConvertToRpcLoginError(LoginError? error) {
      switch (error) {
        case null: return AuthResponse.Types.LoginResult.Success;
        case LoginError.AccountIsAlreadyConnected: return AuthResponse.Types.LoginResult.AccountIsAlreadyConnected;
        case LoginError.InvalidAccount: return AuthResponse.Types.LoginResult.InvalidAccount;
        case LoginError.InvalidPassword: return AuthResponse.Types.LoginResult.InvalidPassword;
        case LoginError.AccountIsLockedOut: return AuthResponse.Types.LoginResult.AccountIsLockedOut;
        default: throw new InvalidEnumArgumentException(nameof(error), (int)error, typeof(LoginError));
      }
    }
  }
}
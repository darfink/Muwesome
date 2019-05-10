using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using log4net;
using Muwesome.DomainModel.Entities;
using Muwesome.Persistence;
using Muwesome.Rpc.LoginServer;
using static Muwesome.Rpc.LoginServer.AuthResponse.Types;

namespace Muwesome.LoginServer.Services {
  internal class AccountAuthService : AccountAuth.AccountAuthBase {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(AccountAuthService));
    private readonly AccountController accountController;
    private readonly CancellationToken cancellationToken;

    /// <summary>Initializes a new instance of the <see cref="AccountAuthService"/> class.</summary>
    public AccountAuthService(
        AccountController accountController,
        CancellationToken cancellationToken) {
      this.accountController = accountController;
      this.cancellationToken = cancellationToken;
    }

    public override async Task RegisterAuthSession(
        IAsyncStreamReader<AuthRequest> requestStream,
        IServerStreamWriter<AuthResponse> responseStream,
        ServerCallContext context) {
      var accountIds = new HashSet<Guid>();
      try {
        while (await requestStream.MoveNext(this.cancellationToken)) {
          if (requestStream.Current.Login != null) {
            var response = await this.Login(requestStream.Current.Login, accountIds);
            await responseStream.WriteAsync(response);
          } else {
            await this.Logout(requestStream.Current.Logout, accountIds);
          }
        }
      } catch (Exception ex) when (!(ex is RpcException)) {
        Logger.Error("An unexpected error occured whilst processing auth requests", ex);
      } finally {
        await this.accountController.LogoutAccountsAsync(accountIds);
      }
    }

    private async Task<AuthResponse> Login(AuthRequest.Types.Login login, ISet<Guid> accountIds) {
      Logger.Debug($"Login attempt for {login.Username}...");
      var accountLoginResult = await this.accountController.LoginAccountAsync(login.Username, login.Password);
      var loginResult = this.ConvertToLoginResult(accountLoginResult);

      return accountLoginResult.Match(
        account => {
          Logger.Debug($"Login successful for {login.Username}...");
          accountIds.Add(account.Id);
          var accountId = ByteString.CopyFrom(account.Id.ToByteArray());
          return new AuthResponse { Result = loginResult, AccountId = accountId };
        },
        error => {
          Logger.Info($"Login failed for {login.Username}; {error}");
          return new AuthResponse { Result = loginResult };
        });
    }

    private async Task Logout(AuthRequest.Types.Logout logout, ISet<Guid> accountIds) {
      var accountId = new Guid(logout.AccountId.ToByteArray());
      await this.accountController.LogoutAccountAsync(accountId);
      accountIds.Remove(accountId);
    }

    private LoginResult ConvertToLoginResult(AccountOrLoginError result) {
      return result.Match(
        account => LoginResult.Success,
        error => {
          switch (error) {
            case LoginError.AlreadyConnected: return LoginResult.AccountIsAlreadyConnected;
            case LoginError.InvalidAccount: return LoginResult.InvalidAccount;
            case LoginError.InvalidPassword: return LoginResult.InvalidPassword;
            case LoginError.LockedOut: return LoginResult.AccountIsLockedOut;
            default: throw new InvalidEnumArgumentException(nameof(error), (int)error, typeof(LoginError));
          }
        });
    }
  }
}
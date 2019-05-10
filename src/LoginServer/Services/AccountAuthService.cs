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
      var loginResult = this.ConvertLoginResult(accountLoginResult.Type);

      if (accountLoginResult.Type == AccountLoginResultType.Success) {
        accountIds.Add(accountLoginResult.Account.Id);
        Logger.Debug($"Login successful for {login.Username}...");

        var accountId = ByteString.CopyFrom(accountLoginResult.Account.Id.ToByteArray());
        return new AuthResponse { Result = loginResult, AccountId = accountId };
      }

      Logger.Info($"Login failed for {login.Username}; {accountLoginResult.Type}");
      return new AuthResponse { Result = loginResult };
    }

    private async Task Logout(AuthRequest.Types.Logout logout, ISet<Guid> accountIds) {
      var accountId = new Guid(logout.AccountId.ToByteArray());
      await this.accountController.LogoutAccountAsync(accountId);
      accountIds.Remove(accountId);
    }

    private LoginResult ConvertLoginResult(AccountLoginResultType type) {
      switch (type) {
        case AccountLoginResultType.Success: return LoginResult.Success;
        case AccountLoginResultType.AlreadyConnected: return LoginResult.AccountIsAlreadyConnected;
        case AccountLoginResultType.InvalidAccount: return LoginResult.InvalidAccount;
        case AccountLoginResultType.InvalidPassword: return LoginResult.InvalidPassword;
        case AccountLoginResultType.LockedOut: return LoginResult.AccountIsLockedOut;
        default: throw new InvalidEnumArgumentException(nameof(type), (int)type, typeof(AccountLoginResultType));
      }
    }
  }
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Muwesome.DomainModel.Entities;
using Muwesome.Persistence;
using Muwesome.Rpc.LoginServer;
using static Muwesome.Rpc.LoginServer.AuthResponse.Types;

namespace Muwesome.LoginServer.Services {
  internal class AccountAuthService : AccountAuth.AccountAuthBase {
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
      } finally {
        await this.accountController.LogoutAccountsAsync(accountIds);
      }
    }

    private async Task<AuthResponse> Login(AuthRequest.Types.Login login, ISet<Guid> accountIds) {
      var accountLoginResult = await this.accountController.LoginAccountAsync(login.Username, login.Password);

      if (accountLoginResult.Success) {
        accountIds.Add(accountLoginResult.Account.Id);
        var accountId = ByteString.CopyFrom(accountLoginResult.Account.Id.ToByteArray());
        return new AuthResponse { Result = LoginResult.Success, AccountId = accountId };
      }

      return new AuthResponse {
        Result = accountLoginResult.InvalidAccount
          ? LoginResult.InvalidAccount
          : accountLoginResult.TimedOut
          ? LoginResult.AccountIsTimedOut
          : accountLoginResult.AlreadyConnected
          ? LoginResult.AccountIsAlreadyConnected
          : LoginResult.InvalidPassword,
      };
    }

    private async Task Logout(AuthRequest.Types.Logout logout, ISet<Guid> accountIds) {
      var accountId = new Guid(logout.AccountId.ToByteArray());
      await this.accountController.LogoutAccountAsync(accountId);
      accountIds.Remove(accountId);
    }
  }
}
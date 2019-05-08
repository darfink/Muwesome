using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Muwesome.Rpc.LoginServer;

namespace Muwesome.LoginServer.Services {
  internal class AccountAuthService : AccountAuth.AccountAuthBase {
    private readonly AccountController accountController;
    private readonly CancellationToken cancellationToken;

    /// <summary>Initializes a new instance of the <see cref="AccountAuthService"/> class.</summary>
    public AccountAuthService(AccountController accountController, CancellationToken cancellationToken) {
      this.accountController = accountController;
      this.cancellationToken = cancellationToken;
    }

    public override async Task RegisterAuthSession(
        IAsyncStreamReader<AuthRequest> requestStream,
        IServerStreamWriter<AuthResponse> responseStream,
        ServerCallContext context) {
      while (await requestStream.MoveNext(this.cancellationToken)) {
        if (requestStream.Current.Login != null) {
          var response = await this.Login(requestStream.Current.Login);
          await responseStream.WriteAsync(response);
        } else {
          await this.Logout(requestStream.Current.Logout);
        }
      }
    }

    private Task<AuthResponse> Login(AuthRequest.Types.Login login) {
      throw new NotImplementedException();
    }

    private Task Logout(AuthRequest.Types.Logout logout) {
      throw new NotImplementedException();
    }
  }
}
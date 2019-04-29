using System;
using System.Threading.Tasks;
using Grpc.Core;
using Muwesome.LoginServer.Rpc;

namespace Muwesome.LoginServer.Services {
  internal class AccountAuthService : AccountAuth.AccountAuthBase {
    private readonly AccountController accountController;

    /// <summary>Initializes a new instance of the <see cref="AccountAuthService"/> class.</summary>
    public AccountAuthService(AccountController accountController) =>
      this.accountController = accountController;

    public override Task<LoginResponse> Login(LoginRequest request, ServerCallContext context) {
      throw new NotImplementedException();
    }

    public override Task<LogoutResponse> Logout(LogoutRequest request, ServerCallContext context) {
      throw new NotImplementedException();
    }
  }
}
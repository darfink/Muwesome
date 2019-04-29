using System;
using System.Threading.Tasks;
using Grpc.Core;
using Muwesome.LoginServer.Rpc;

namespace Muwesome.LoginServer.Services {
  public class AccountAuthService : AccountAuth.AccountAuthBase {
    public override Task<LoginResponse> Login(LoginRequest request, ServerCallContext context) {
      throw new NotImplementedException();
    }

    public override Task<LogoutResponse> Logout(LogoutRequest request, ServerCallContext context) {
      throw new NotImplementedException();
    }
  }
}
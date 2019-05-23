using System.ComponentModel;
using System.Threading.Tasks;
using Muwesome.Common;
using Muwesome.GameLogic;
using Muwesome.GameLogic.PlayerActions;

namespace Muwesome.GameServer {
  internal class LoginServiceAdapter : ILoginService {
    private readonly IAccountLoginService accountLoginService;

    /// <summary>Initializes a new instance of the <see cref="LoginServiceAdapter"/> class.</summary>
    public LoginServiceAdapter(IAccountLoginService accountLoginService) =>
      this.accountLoginService = accountLoginService;

    /// <inheritdoc />
    public async Task<LoginResult> TryLoginAsync(string username, string password) {
      var loginError = await this.accountLoginService.TryLoginAsync(username, password);
      return this.ConvertLoginErrorToResult(loginError);
    }

    /// <inheritdoc />
    public Task<bool> TryLogoutAsync(string username) => this.accountLoginService.TryLogoutAsync(username);

    private LoginResult ConvertLoginErrorToResult(LoginError? error) {
      switch (error) {
        case null: return LoginResult.Success;
        case LoginError.InvalidAccount: return LoginResult.InvalidAccount;
        case LoginError.InvalidPassword: return LoginResult.InvalidPassword;
        case LoginError.AccountIsBlocked: return LoginResult.AccountIsBlocked;
        case LoginError.AccountIsLockedOut: return LoginResult.TooManyFailedLoginAttempts;
        case LoginError.AccountIsAlreadyConnected: return LoginResult.AccountIsAlreadyConnected;
        default: throw new InvalidEnumArgumentException(nameof(error), (int)error, typeof(LoginError));
      }
    }
  }
}
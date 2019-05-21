using System;
using log4net;
using Muwesome.MethodDelegate;

namespace Muwesome.GameLogic.Actions.Players.Handlers {
  /// <summary>A login action handler.</summary>
  internal class LoginActionHandler {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(LoginActionHandler));
    private readonly ILoginService loginService;

    /// <summary>Initializes a new instance of the <see cref="LoginActionHandler"/> class.</summary>
    public LoginActionHandler(ILoginService loginService) => this.loginService = loginService;

    /// <summary>Logs a player into the game.</summary>
    [MethodDelegate(typeof(LoginAction))]
    public async void Login([Inject] Player player, string username, string password) {
      // TODO: Handle asynchronous exceptions
      Logger.Info($"Login with {username}");
      var result = await this.loginService.TryLoginAsync(username, password);

      if (result == LoginResult.Success) {
        player.Account = await player.PersistenceContext.GetAccountByUsernameAsync(username);

        if (player.Account == null) {
          Logger.Error($"Account returned by login service ({username}) was null");
          result = LoginResult.InternalError;
        }
      }

      player.Action<ShowLoginResultAction>()?.Invoke(result);
    }

    /// <summary>Logs a player out of the game.</summary>
    [MethodDelegate(typeof(LogoutAction))]
    public void Logout([Inject] Player player, LogoutType logoutType) {
    }
  }
}
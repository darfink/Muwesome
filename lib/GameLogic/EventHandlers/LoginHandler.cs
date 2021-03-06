using System;
using log4net;
using Muwesome.GameLogic.Interface;
using Muwesome.GameLogic.Interface.Actions;
using Muwesome.GameLogic.Interface.Events;
using Muwesome.MethodDelegate;

namespace Muwesome.GameLogic.EventHandlers {
  /// <summary>A player login handler.</summary>
  internal class LoginHandler {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(LoginHandler));
    private readonly ILoginService loginService;

    /// <summary>Initializes a new instance of the <see cref="LoginHandler"/> class.</summary>
    public LoginHandler(ILoginService loginService) => this.loginService = loginService;

    /// <summary>Logs a player into the game.</summary>
    [MethodDelegate(typeof(OnLogin))]
    public async void Login(Player player, string username, string password) {
      // TODO: Handle asynchronous exceptions
      Logger.InfoFormat("Login with {0}", username);
      var result = await this.loginService.TryLoginAsync(username, password);

      if (result == LoginResult.Success) {
        player.Account = await player.PersistenceContext.GetAccountByUsernameAsync(username);

        if (player.Account == null) {
          Logger.ErrorFormat("Account returned by login service ({0}) was null", username);
          result = LoginResult.InternalError;
        }
      }

      player.Action<ShowLoginResult>()?.Invoke(result);
    }

    /// <summary>Logs a player out of the game.</summary>
    [MethodDelegate(typeof(OnLogout))]
    public void Logout(Player player, LogoutType logoutType) {
    }
  }
}
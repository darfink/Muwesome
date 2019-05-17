using System;
using log4net;

namespace Muwesome.GameLogic.Actions.Players.Handlers {
  /// <summary>A login action handler.</summary>
  internal class LoginActionHandler :
      IActionProvider<Player, LoginAction>,
      IActionProvider<Player, LogoutAction> {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(LoginActionHandler));
    private readonly ILoginService loginService;

    /// <summary>Initializes a new instance of the <see cref="LoginActionHandler"/> class.</summary>
    public LoginActionHandler(ILoginService loginService) => this.loginService = loginService;

    /// <inheritdoc />
    LoginAction IActionProvider<Player, LoginAction>.CreateAction(Player player) => (username, password) => this.Login(player, username, password);

    /// <inheritdoc />
    LogoutAction IActionProvider<Player, LogoutAction>.CreateAction(Player player) => (logoutType) => this.Logout(player, logoutType);

    /// <summary>Logs a player into the game.</summary>
    // TODO: Handle asynchronous exceptions
    private async void Login(Player player, string username, string password) {
      Logger.Info($"Login with {username} & {password}");
      var result = await this.loginService.TryLoginAsync(username, password);

      if (result == LoginResult.Success) {
      }

      player.Action<ShowLoginResultAction>()?.Invoke(result);
    }

    /// <summary>Logs a player out of the game.</summary>
    private void Logout(Player player, LogoutType logoutType) {
    }
  }
}
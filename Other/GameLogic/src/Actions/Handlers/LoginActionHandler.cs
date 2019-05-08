using System;
using log4net;

namespace Muwesome.GameLogic.Actions.Handlers {
  /// <summary>A login action handler.</summary>
  internal class LoginActionHandler :
      IPlayerActionProvider<LoginAction>,
      IPlayerActionProvider<LogoutAction> {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(LoginActionHandler));

    /// <summary>Initializes a new instance of the <see cref="LoginActionHandler"/> class.</summary>
    public LoginActionHandler() {
    }

    /// <inheritdoc />
    LoginAction IPlayerActionProvider<LoginAction>.CreateAction(Player player) =>
      (username, password) => this.Login(player, username, password);

    /// <inheritdoc />
    LogoutAction IPlayerActionProvider<LogoutAction>.CreateAction(Player player) =>
      (logoutType) => this.Logout(player, logoutType);

    /// <summary>Logs a player into the game.</summary>
    private void Login(Player player, string username, string password) {
      Logger.Info($"Login with {username} & {password}");
      player.Action<ShowLoginResultAction>()?.Invoke(LoginResult.AccountIsBlocked);
    }

    /// <summary>Logs a player out of the game.</summary>
    private void Logout(Player player, LogoutType logoutType) {
    }
  }
}
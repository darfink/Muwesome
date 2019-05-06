using System;

namespace Muwesome.GameLogic.Actions.Handlers {
  /// <summary>A login action handler.</summary>
  internal class LoginActionHandler :
      IPlayerActionFactory<LoginAction>,
      IPlayerActionFactory<LogoutAction> {
    /// <inheritdoc />
    LoginAction IPlayerActionFactory<LoginAction>.CreateAction(Player player) =>
      (username, password) => this.Login(player, username, password);

    /// <inheritdoc />
    LogoutAction IPlayerActionFactory<LogoutAction>.CreateAction(Player player) =>
      (logoutType) => this.Logout(player, logoutType);

    /// <summary>Logs a player into the game.</summary>
    private void Login(Player player, string username, string password) {
    }

    /// <summary>Logs a player out of the game.</summary>
    private void Logout(Player player, LogoutType logoutType) {
    }
  }
}
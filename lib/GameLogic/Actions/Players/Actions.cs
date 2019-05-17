namespace Muwesome.GameLogic.Actions.Players {
  /// <summary>Logs the player into the game.</summary>
  public delegate void LoginAction(string username, string password);

  /// <summary>Logs the player out of the game.</summary>
  public delegate void LogoutAction(LogoutType logoutType);

  // -------------------------------------------------------

  /// <summary>Displays the player's login window.</summary>
  public delegate void ShowLoginWindowAction();

  /// <summary>Displays the login result to the player.</summary>
  public delegate void ShowLoginResultAction(LoginResult result);

  /// <summary>Displays a message to the player.</summary>
  public delegate void ShowMessageAction(string message);
}
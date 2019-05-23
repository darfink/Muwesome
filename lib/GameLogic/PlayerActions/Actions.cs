namespace Muwesome.GameLogic.PlayerActions {
  /// <summary>Logs the player into the game.</summary>
  public delegate void LoginAction(string username, string password);

  /// <summary>Logs the player out of the game.</summary>
  public delegate void LogoutAction(LogoutType logoutType);

  /// <summary>Requests the player's character list.</summary>
  public delegate void RequestCharactersAction();

  // -------------------------------------------------------

  /// <summary>Displays the player's login window.</summary>
  public delegate void ShowLoginWindowAction();

  /// <summary>Displays the login result to the player.</summary>
  public delegate void ShowLoginResultAction(LoginResult result);

  /// <summary>Displays a message to the player.</summary>
  public delegate void ShowMessageAction(string message);

  /// <summary>Displays the character list to the player.</summary>
  public delegate void ShowCharactersAction();
}
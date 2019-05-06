namespace Muwesome.GameLogic.Actions {
  /// <summary>Represents all possible logout types.</summary>
  public enum LogoutType : byte {
    /// <summary>Exits the player from the game.</summary>
    ExitGame,
    /// <summary>Returns the player to character select.</summary>
    BackToCharacterSelect,
    /// <summary>Returns the player to server select.</summary>
    BackToServerSelect,
  }
}
namespace Muwesome.GameServer {
  /// <summary>A connect server registerer.</summary>
  public interface IConnectServerRegisterer {
    /// <summary>Gets a value indicating whether the game server is currently registered or not.</summary>
    bool IsRegistered { get; }
  }
}
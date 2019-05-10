using System.Threading.Tasks;

namespace Muwesome.GameServer {
  /// <summary>A game server registerer.</summary>
  public interface IGameServerRegisterer {
    /// <summary>Gets the registerer's task.</summary>
    Task ShutdownTask { get; }

    /// <summary>Gets a value indicating whether the game server is currently registered or not.</summary>
    bool IsRegistered { get; }
  }
}
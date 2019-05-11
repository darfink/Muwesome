using System.Threading.Tasks;

namespace Muwesome.Interfaces {
  /// <summary>A game server registrar.</summary>
  /// <remarks>This is implemented by the <see cref="ConnectServer" />.</remarks>
  public interface IGameServerRegistrar {
    /// <summary>Gets the number of servers registered.</summary>
    int GameServersRegistered { get; }

    /// <summary>Registers a new game server</summary>
    Task RegisterGameServerAsync(GameServerInfo server);

    /// <summary>Deregisters an existing game server</summary>
    // TODO: Special class for server codes.
    Task DeregisterGameServerAsync(ushort serverCode);
  }
}
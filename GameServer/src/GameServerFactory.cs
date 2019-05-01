using Muwesome.Network;

namespace Muwesome.GameServer {
  /// <summary>A factory for game servers.</summary>
  public static class GameServerFactory {
    /// <summary>Initializes a new instance of the <see cref="GameServer" /> class with default implementations.</summary>
    public static GameServer Create(Configuration config) {
      var clientListener = new ClientTcpListener(config.MaxPacketSize);

      return new GameServer(config);
    }
  }
}
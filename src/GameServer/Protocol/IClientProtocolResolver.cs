using Muwesome.Protocol.Game;

namespace Muwesome.GameServer.Protocol {
  /// <summary>A client protocol resolver.</summary>
  internal interface IClientProtocolResolver {
    /// <summary>Resolves a client version to a compatible protocol.</summary>
    ClientProtocol Resolve(ClientVersion clientVersion);
  }
}
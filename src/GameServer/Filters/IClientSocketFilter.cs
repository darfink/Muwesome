using System;
using System.Net.Sockets;

namespace Muwesome.GameServer.Filters {
  /// <summary>A client filter.</summary>
  internal interface IClientSocketFilter {
    /// <summary>Called after a client socket has been accepted.</summary>
    /// <returns>True if the client is allowed to connect.</returns>
    bool OnAllowClientSocketAccept(Socket socket);
  }
}
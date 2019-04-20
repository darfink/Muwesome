using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Muwesome.Network {
  /// <summary>A delegate which is executed whenever a socket is accepted.</summary>
  public delegate void SocketAcceptedHandler(Socket socket);

  public static class TcpListenerExtensions {
    public static async Task AcceptIncomingSocketsAsync(this TcpListener listener, SocketAcceptedHandler socketAcceptedHandler) {
      Socket socket;
      try {
        socket = await listener.AcceptSocketAsync();
      } catch (ObjectDisposedException) {
        return;
      }

      socketAcceptedHandler(socket);

      if (listener.Server.IsBound) {
        await listener.AcceptIncomingSocketsAsync(socketAcceptedHandler);
      }
    }
  }
}
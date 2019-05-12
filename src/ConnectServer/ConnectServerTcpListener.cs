using System.Net;
using Muwesome.Network;
using Muwesome.Network.Tcp;
using Muwesome.Protocol;

namespace Muwesome.ConnectServer {
  /// <summary>A connect server TCP listener from incoming clients.</summary>
  public class ConnectServerTcpListener : ClientTcpListenerBase<Client> {
    private readonly IPacketHandler<Client> clientProtocol;

    /// <summary>Initializes a new instance of the <see cref="ConnectServerTcpListener" /> class.</summary>
    // TODO: The determined end point should be injected
    public ConnectServerTcpListener(
        Configuration config,
        IPacketHandler<Client> clientProtocol)
        : base(config.ClientListenerEndPoint, config.MaxPacketSize) {
      this.clientProtocol = clientProtocol;
    }

    /// <inheritdoc />
    protected override Client OnConnectionEstablished(IConnection connection) =>
      new Client(connection, this.clientProtocol);
  }
}
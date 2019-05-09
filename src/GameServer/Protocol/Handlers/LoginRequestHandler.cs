using System;
using log4net;
using Muwesome.GameLogic.Actions;
using Muwesome.Packet.Utility;
using Muwesome.Protocol;
using Muwesome.Protocol.Game.Client;

namespace Muwesome.GameServer.Protocol.Handlers {
  /// <summary>A packet handler for incoming login requests.</summary>
  internal class LoginRequestHandler : IPacketHandler<Client> {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(LoginRequestHandler));

    /// <inheritdoc />
    public bool HandlePacket(Client client, Span<byte> packet) {
      ref var login = ref PacketHelper.ParsePacket<LoginRequest>(packet);

      if (login.Version != client.Version) {
        Logger.Info($"Client version mismatch for {client}; expected {client.Version}, was {login.Version}");
        client.Player.Action<ShowLoginResultAction>()?.Invoke(LoginResult.InvalidGameVersion);
      } else if (!login.Serial.SequenceEqual(client.Serial)) {
        // TODO: Print serial ASCII escaped?
        Logger.Info($"Client serial mismatch for {client}; expected {client.Serial}, was {login.Serial.ToArray()}");
        client.Connection.Disconnect();
      } else {
        client.Player.Action<LoginAction>()?.Invoke(login.Username, login.Password);
      }

      return true;
    }
  }
}
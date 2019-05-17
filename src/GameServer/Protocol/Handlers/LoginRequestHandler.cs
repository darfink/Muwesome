using System;
using log4net;
using Muwesome.GameLogic.Actions.Players;
using Muwesome.Packet.Utility;
using Muwesome.Protocol;
using Muwesome.Protocol.Game.Client;

namespace Muwesome.GameServer.Protocol.Handlers {
  /// <summary>A packet handler for incoming login requests.</summary>
  internal class LoginRequestHandler : PacketHandler<LoginRequest> {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(LoginRequestHandler));

    /// <summary>Gets or sets a value indicating whether client serials are validated or not.</summary>
    public bool ValidateClientSerial { get; set; }

    /// <inheritdoc />
    public override bool HandlePacket(Client client, Span<byte> packet) {
      ref var login = ref PacketHelper.ParsePacket<LoginRequest>(packet);

      if (login.Version != client.Version) {
        Logger.Info($"Client version mismatch for {client}; expected {client.Version}, was {login.Version}");
        client.Player.Action<ShowLoginResultAction>()?.Invoke(LoginResult.InvalidGameVersion);
        return true;
      }

      bool isSerialInvalid = this.ValidateClientSerial && login.Serial.SequenceEqual(client.Serial);

      if (isSerialInvalid) { // TODO: Print serial ASCII escaped?
        Logger.Info($"Client serial mismatch for {client}; expected {client.Serial}, was {login.Serial.ToArray()}");
        client.Connection.Disconnect();
        return true;
      }

      client.Player.Action<LoginAction>()?.Invoke(login.Username, login.Password);
      return true;
    }
  }
}
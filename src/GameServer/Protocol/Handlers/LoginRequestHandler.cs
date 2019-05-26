using System;
using log4net;
using Muwesome.GameLogic.Interface.Actions;
using Muwesome.GameLogic.Interface.Events;
using Muwesome.Packet.Utility;
using Muwesome.Protocol;
using Muwesome.Protocol.Game.Client;

namespace Muwesome.GameServer.Protocol.Handlers {
  /// <summary>A packet handler for incoming login requests.</summary>
  [ProtocolPacket(typeof(LoginRequest))]
  internal class LoginRequestHandler : PacketHandler, IClientSerialValidator {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(LoginRequestHandler));

    /// <inheritdoc />
    public bool ValidateClientSerial { get; set; }

    /// <inheritdoc />
    public override bool HandlePacket(Client client, Span<byte> packet) {
      ref var login = ref PacketHelper.ParsePacket<LoginRequest>(packet);

      if (login.Version != client.Protocol.Version) {
        Logger.InfoFormat("Client version mismatch for {0}; expected {1}, was {2}", client, client.Protocol.Version, login.Version);
        client.Player.Action<ShowLoginResult>()?.Invoke(LoginResult.InvalidGameVersion);
        return true;
      }

      bool isSerialInvalid = this.ValidateClientSerial && login.Serial.SequenceEqual(client.Serial);

      if (isSerialInvalid) { // TODO: Print serial ASCII escaped?
        Logger.InfoFormat("Client serial mismatch for {0}; expected {1}, was {2}", client, client.Serial, login.Serial.ToArray());
        client.Connection.Disconnect();
        return true;
      }

      this.PlayerEventDispatcher<OnLogin>().Invoke(client.Player, login.Username, login.Password);
      return true;
    }
  }
}
using System;
using log4net;
using Muwesome.GameLogic.Interface.Events;
using Muwesome.Packet.Utility;
using Muwesome.Protocol;
using Muwesome.Protocol.Game.Client;

namespace Muwesome.GameServer.Protocol.Handlers.Character {
  /// <summary>A packet handler for incoming login requests.</summary>
  [ProtocolPacket(typeof(CharacterListRequest))]
  internal class CharacterListRequestHandler : PacketHandler {
    /// <inheritdoc />
    public override bool HandlePacket(Client client, Span<byte> packet) {
      this.PlayerEventDispatcher<OnRequestCharacters>().Invoke(client.Player);
      return true;
    }
  }
}
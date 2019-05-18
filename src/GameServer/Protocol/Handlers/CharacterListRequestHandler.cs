using System;
using log4net;
using Muwesome.GameLogic.Actions.Players;
using Muwesome.Packet.Utility;
using Muwesome.Protocol;
using Muwesome.Protocol.Game.Client;

namespace Muwesome.GameServer.Protocol.Handlers {
  /// <summary>A packet handler for incoming login requests.</summary>
  internal class CharacterListRequestHandler : PacketHandler<CharacterListRequest> {
    /// <inheritdoc />
    public override bool HandlePacket(Client client, Span<byte> packet) {
      client.Player.Action<RequestCharactersAction>()?.Invoke();
      return true;
    }
  }
}
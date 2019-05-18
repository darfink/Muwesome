using System;
using Muwesome.GameLogic;
using Muwesome.GameLogic.Actions.Players;
using Muwesome.Network;
using Muwesome.Packet.Utility;
using Muwesome.Protocol.Game.Server;

namespace Muwesome.GameServer.Protocol.Dispatchers {
  /// <summary>A packet dispatcher for login results.</summary>
  internal class CharacterListDispatcher : PacketDispatcher<CharacterList, ShowCharactersAction> {
    /// <inheritdoc />
    protected override ShowCharactersAction CreateDispatcherForAction(Client client) => () => this.SendCharacterList(client);

    /// <summary>Sends the login result to a client.</summary>
    private void SendCharacterList(Client client) {
      var characterCount = client.Player.Account.Characters.Count;
      var packetSize = PacketHelper.GetPacketSize<CharacterList, CharacterList.Character>(characterCount);

      using (var writer = client.Connection.StartWrite(packetSize)) {
        PacketHelper.CreatePacket<CharacterList, CharacterList.Character>(characterCount, writer.Span, out Span<CharacterList.Character> characters);
        characters[0].Level = 3;
        characters[0].Name = "n00b";
      }
    }
  }
}
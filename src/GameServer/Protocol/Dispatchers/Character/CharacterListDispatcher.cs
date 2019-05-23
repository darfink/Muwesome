using System;
using Muwesome.GameLogic;
using Muwesome.GameLogic.PlayerActions;
using Muwesome.MethodDelegate;
using Muwesome.Network;
using Muwesome.Packet.Utility;
using Muwesome.Protocol.Game.Server;

namespace Muwesome.GameServer.Protocol.Dispatchers.Character {
  /// <summary>A packet dispatcher for login results.</summary>
  [ProtocolPacket(typeof(CharacterList))]
  internal class CharacterListDispatcher : PacketDispatcher {
    /// <summary>Sends the login result to a client.</summary>
    [MethodDelegate(typeof(ShowCharactersAction))]
    public void SendCharacterList([Inject] Client client) {
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
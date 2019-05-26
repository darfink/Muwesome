using System;
using Muwesome.GameLogic.Interface.Actions;
using Muwesome.GameServer.Utility;
using Muwesome.MethodDelegate;
using Muwesome.Network;
using Muwesome.Packet.Utility;
using Muwesome.Protocol.Game.Server;

namespace Muwesome.GameServer.Protocol.Dispatchers.Character {
  /// <summary>A packet dispatcher for character lists.</summary>
  [ProtocolPacket(typeof(CharacterList))]
  internal class CharacterListDispatcher : PacketDispatcher {
    /// <summary>Sends the character list to a client.</summary>
    [MethodDelegate(typeof(ShowCharacters))]
    public void SendCharacterList([Inject] Client client) {
      var characterCount = client.Player.Account.Characters.Count;
      var packetSize = PacketHelper.GetPacketSize<CharacterList, CharacterList.Character>(characterCount);

      using (var writer = client.Connection.StartWrite(packetSize)) {
        PacketHelper.CreatePacket<CharacterList, CharacterList.Character>(characterCount, writer.Span, out Span<CharacterList.Character> characters);

        foreach (var (i, character) in client.Player.Account.Characters.Enumerate()) {
          characters[i].Name = character.Name;
          characters[i].Slot = character.Slot;
        }
      }
    }
  }
}
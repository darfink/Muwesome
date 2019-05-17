using System.Runtime.InteropServices;
using Muwesome.Packet;
using Muwesome.Packet.Utility;

namespace Muwesome.Protocol.Game.Client {
  [Packet(0xC1, 0xF3, 0x00)]
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct CharacterListRequest : IFixedPacket {
  }
}
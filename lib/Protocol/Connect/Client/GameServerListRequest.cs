using System.Runtime.InteropServices;
using Muwesome.Packet;
using Muwesome.Packet.Utility;
using Muwesome.Protocol.Utility;

namespace Muwesome.Protocol.Connect.Client {
  [Packet(0xC1, 0xF4, 0x06)]
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct GameServerListRequest : IFixedPacket {
  }
}
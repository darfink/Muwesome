using System.Runtime.InteropServices;
using Muwesome.Packet;
using Muwesome.Packet.Utility;
using Muwesome.Protocol.Utility;

namespace Muwesome.Protocol.Connect.Client {
  [Packet(0xC1, 0xF4, 0x03)]
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct GameServerInfoRequest : IFixedPacket {
    public LittleEndian<ushort> ServerCode;
  }
}
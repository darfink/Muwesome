using System.Runtime.InteropServices;
using Muwesome.Packet;
using Muwesome.Packet.Utility;
using Muwesome.Protocol.Utility;

namespace Muwesome.Protocol.Connect.V20050502 {
  [Packet(0xC1, 0xA9)]
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct ClientUpdateRequest : IFixedPacket {
    // NOTE: This packet, unlike the others, is XOR encrypted
    public byte Major;
    public byte Minor;
    public byte Patch;
  }

  [Packet(0xC1, 0xF4, 0x03)]
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct GameServerInfoRequest : IFixedPacket {
    public LittleEndian<ushort> ServerCode;
  }

  [Packet(0xC1, 0xF4, 0x06)]
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct GameServerListRequest : IFixedPacket { }
}
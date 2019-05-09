using System.Runtime.InteropServices;
using Muwesome.Packet;
using Muwesome.Packet.Utility;
using Muwesome.Protocol.Utility;

namespace Muwesome.Protocol.Game.Client {
  [Packet(0xC1, 0x0E, 0x00)]
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct ClientTime : IFixedPacket {
    public LittleEndian<uint> TickCount;
    public LittleEndian<ushort> AttackSpeed;
    public LittleEndian<ushort> MagicSpeed;
  }
}
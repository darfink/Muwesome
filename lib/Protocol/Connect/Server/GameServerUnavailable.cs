using System;
using System.Runtime.InteropServices;
using System.Text;
using Muwesome.Packet;
using Muwesome.Packet.Utility;
using Muwesome.Protocol.Utility;

namespace Muwesome.Protocol.Connect.Server {
  [Packet(0xC1, 0xF4, 0x05)]
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct GameServerUnavailable : IFixedPacket {
    public LittleEndian<ushort> ServerCode;
  }
}
using System;
using System.Runtime.InteropServices;
using Muwesome.Packet;
using Muwesome.Packet.Utility;
using Muwesome.Protocol.Utility;

namespace Muwesome.Protocol.Game.V1_02_03 {
  [Packet(0xC1, 0xF1, 0x00)]
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct JoinResult : IFixedPacket {
    [MarshalAs(UnmanagedType.I1)]
    public bool Success;
    public BigEndian<ushort> PlayerId;
    public Version ClientVersion;
  }
}
using System;
using System.Runtime.InteropServices;
using Muwesome.Packet;
using Muwesome.Packet.Utility;
using Muwesome.Protocol.Utility;

namespace Muwesome.Protocol.Game.Server {
  [Packet(0xC1, 0xF1, 0x01)]
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct LoginResult : IFixedPacket {
    public byte Result;
  }
}
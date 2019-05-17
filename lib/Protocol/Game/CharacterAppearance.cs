using System;
using System.Runtime.InteropServices;
using Muwesome.Packet;
using Muwesome.Packet.Utility;

namespace Muwesome.Protocol.Game {
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct CharacterAppearance {
    private unsafe fixed byte data[18];
  }
}
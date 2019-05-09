using System.Runtime.InteropServices;
using Muwesome.Packet;
using Muwesome.Packet.Utility;
using Muwesome.Protocol.Utility;

namespace Muwesome.Protocol.Connect.Client {
  [Packet(0xC1, 0xA9)]
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct ClientUpdateRequest : IFixedPacket {
    // NOTE: This packet, unlike the others, is XOR encrypted
    public byte Major;
    public byte Minor;
    public byte Patch;
  }
}
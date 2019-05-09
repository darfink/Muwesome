using System;
using System.Runtime.InteropServices;
using System.Text;
using Muwesome.Packet;
using Muwesome.Packet.Utility;
using Muwesome.Protocol.Utility;

#pragma warning disable SA1202
#pragma warning disable SA1501
#pragma warning disable SA1503

namespace Muwesome.Protocol.Connect.Server {
  [Packet(0xC1, 0xF4, 0x03)]
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct GameServerInfo : IFixedPacket {
    private unsafe fixed byte host[16];
    public LittleEndian<ushort> Port;

    public string Host {
      get { unsafe { fixed (byte* data = this.host) return PointerHelper.GetString(data, 16, Encoding.ASCII); } }
      set { unsafe { fixed (byte* data = this.host) PointerHelper.SetString(data, 16, value, Encoding.ASCII); } }
    }
  }
}
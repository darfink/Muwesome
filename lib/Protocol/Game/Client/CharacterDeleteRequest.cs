using System.Runtime.InteropServices;
using System.Text;
using Muwesome.Packet;
using Muwesome.Packet.Utility;
using Muwesome.Protocol.Utility;

#pragma warning disable SA1202
#pragma warning disable SA1501
#pragma warning disable SA1503

namespace Muwesome.Protocol.Game.Client {
  [Packet(0xC1, 0xF3, 0x02)]
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct CharacterDeleteRequest : IFixedPacket {
    private unsafe fixed byte username[10];
    private unsafe fixed byte securityCode[10];

    public string Username {
      get { unsafe { fixed (byte* data = this.username) return PointerHelper.GetString(data, 10, Encoding.ASCII); } }
      set { unsafe { fixed (byte* data = this.username) PointerHelper.SetString(data, 10, value, Encoding.ASCII); } }
    }

    public string SecurityCode {
      get { unsafe { fixed (byte* data = this.securityCode) return PointerHelper.GetString(data, 10, Encoding.ASCII); } }
      set { unsafe { fixed (byte* data = this.securityCode) PointerHelper.SetString(data, 10, value, Encoding.ASCII); } }
    }
  }
}
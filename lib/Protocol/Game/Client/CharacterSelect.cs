using System.Runtime.InteropServices;
using System.Text;
using Muwesome.Packet;
using Muwesome.Packet.Utility;
using Muwesome.Protocol.Utility;

#pragma warning disable SA1202
#pragma warning disable SA1501
#pragma warning disable SA1503

namespace Muwesome.Protocol.Game.Client {
  [Packet(0xC1, 0xF3, 0x03)]
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct CharacterSelect : IFixedPacket {
    private unsafe fixed byte name[10];

    public string Name {
      get { unsafe { fixed (byte* data = this.name) return PointerHelper.GetString(data, 10, Encoding.ASCII); } }
      set { unsafe { fixed (byte* data = this.name) PointerHelper.SetString(data, 10, value, Encoding.ASCII); } }
    }
  }
}
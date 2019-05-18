using System.Runtime.InteropServices;
using System.Text;
using Muwesome.Packet;
using Muwesome.Packet.Utility;
using Muwesome.Protocol.Utility;

#pragma warning disable SA1202
#pragma warning disable SA1501
#pragma warning disable SA1503

namespace Muwesome.Protocol.Game.Client {
  [Packet(0xC1, 0xF3, 0x01)]
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct CharacterCreateRequest : IFixedPacket {
    private unsafe fixed byte name[10];
    private byte @class;

    public byte Class {
      get => (byte)(this.@class >> 4);
      set => this.@class = (byte)((value << 5) | ((value & 0x08) << 1));
    }

    public string Name {
      get { unsafe { fixed (byte* data = this.name) return PointerHelper.GetString(data, 10, Encoding.ASCII); } }
      set { unsafe { fixed (byte* data = this.name) PointerHelper.SetString(data, 10, value, Encoding.ASCII); } }
    }
  }
}
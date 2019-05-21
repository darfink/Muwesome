using System;
using System.Runtime.InteropServices;
using System.Text;
using Muwesome.Packet;
using Muwesome.Packet.Utility;
using Muwesome.Protocol.Utility;

#pragma warning disable SA1202
#pragma warning disable SA1214
#pragma warning disable SA1501
#pragma warning disable SA1503

namespace Muwesome.Protocol.Game.Server {
  [Packet(0xC1, 0xF3, 0x00)]
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct CharacterList : IDynamicPacket<CharacterList.Character> {
    public byte MaximumClassAvailable;
    private byte teleportReset; // TODO:
    private byte count;

    public int Count {
      get => this.count;
      set => this.count = checked((byte)value);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Character : IInitializable {
      public byte Slot;
      private unsafe fixed byte name[10];
      private readonly byte padding;
      public LittleEndian<ushort> Level;
      public byte Status;
      public CharacterAppearance Appearance;
      public byte GuildRole;

      public string Name {
        get { unsafe { fixed (byte* data = this.name) return PointerHelper.GetString(data, 10, Encoding.ASCII); } }
        set { unsafe { fixed (byte* data = this.name) PointerHelper.SetString(data, 10, value, Encoding.ASCII); } }
      }

      public void Initialize() => this.Appearance.Initialize();
    }
  }
}
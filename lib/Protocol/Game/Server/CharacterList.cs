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
  [Packet(0xC2, 0xF4, 0x06)]
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct CharacterList : IDynamicPacket<CharacterList.Character> {
    public byte MaximumClassAvailable;
    public bool PositionHasBeenReset;
    private byte count;

    public int Count {
      get => this.count;
      set => this.count = checked((byte)value);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Character : IInitializable {
      public byte Slot;
      private unsafe fixed byte username[10];
      private readonly byte padding;
      public LittleEndian<ushort> Level;
      public byte Status;
      public CharacterAppearance Appearance;
      public byte GuildRole;

      public string Username {
        get { unsafe { fixed (byte* data = this.username) return PointerHelper.GetString(data, 10, Encoding.ASCII); } }
        set { unsafe { fixed (byte* data = this.username) PointerHelper.SetString(data, 10, value, Encoding.ASCII); } }
      }

      public void Initialize() => this.Appearance.Initialize();
    }
  }
}
using System;
using System.Runtime.InteropServices;
using System.Text;
using Muwesome.Packet;
using Muwesome.Packet.Utility;
using Muwesome.Protocol.Utility;

namespace Muwesome.Protocol.Connect.Server {
  [Packet(0xC2, 0xF4, 0x06)]
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct GameServerList : IDynamicPacket<GameServerList.GameServer> {
    private BigEndian<ushort> serverCount;

    public int Count {
      get => this.serverCount;
      set => this.serverCount = checked((ushort)value);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GameServer : IInitializable {
      public LittleEndian<ushort> Code;
      private byte load;
      private byte unused;

      public float Load {
        get => (this.load & 0x7F) / 100.0f;
        set => this.load = (byte)((int)(Math.Max(Math.Min(value, 1f), 0f) * 100.0f) | (this.load & 0x80));
      }

      public bool IsPreparing {
        get => (this.load & 0x80) > 0;
        set => this.load = (byte)(value ? (this.load | 0x80) : (this.load & ~0x80));
      }

      public void Initialize() => this.unused = 0x77;
    }
  }
}
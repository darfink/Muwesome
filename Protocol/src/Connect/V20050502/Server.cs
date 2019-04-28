using System;
using System.Text;
using System.Runtime.InteropServices;
using Muwesome.Packet;
using Muwesome.Packet.Utility;
using Muwesome.Protocol.Utility;

namespace Muwesome.Protocol.Connect.V20050502 {
  [Packet(0xC1, 0x00)]
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct ConnectResult : IFixedPacket {
    [MarshalAs(UnmanagedType.I1)]
    public bool Success;
  }

  [Packet(0xC1, 0xF4, 0x03)]
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct GameServerInfo : IFixedPacket {
    private unsafe fixed byte host[16];
    public LittleEndian<ushort> Port;

    public string Host {
      get { unsafe { fixed (byte* data = host) return PointerHelper.GetString(data, 16, Encoding.ASCII); } }
      set { unsafe { fixed (byte* data = host) PointerHelper.SetString(data, 16, value, Encoding.ASCII); } }
    }
  }

  [Packet(0xC1, 0xF4, 0x05)]
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct GameServerUnavailable : IFixedPacket {
    public LittleEndian<ushort> ServerCode;
  }

  [Packet(0xC2, 0xF4, 0x06)]
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct GameServerList : IDynamicPacket<GameServerList.GameServer> {
    private BigEndian<ushort> serverCount;

    public int Count {
      get => serverCount;
      set => serverCount = checked((ushort)value);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GameServer {
      public LittleEndian<ushort> Code;
      private byte load;
      private byte unused;

      public float Load {
        get => (load & 0x7F) / 100.0f;
        set => load = (byte)((int)(Math.Max(Math.Min(value, 1f), 0f) * 100.0f) | (load & 0x80));
      }

      public bool IsPreparing {
        get => (load & 0x80) > 0;
        set => load = (byte)(value ? (load | 0x80) : (load & ~0x80));
      }
    }
  }
}
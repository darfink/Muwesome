using System;
using System.Runtime.InteropServices;
using Muwesome.Packet;
using Muwesome.Packet.Utility;
using Muwesome.Protocol.Utility;

#pragma warning disable SA1501
#pragma warning disable SA1503

namespace Muwesome.Protocol.Game.Server {
  [Packet(0xC1, 0xF1, 0x00)]
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct JoinResult : IFixedPacket {
    [MarshalAs(UnmanagedType.I1)]
    public bool Success;
    public BigEndian<ushort> PlayerId;
    private unsafe fixed byte version[5];

    public ClientVersion Version {
      get { unsafe { fixed (byte* data = this.version) return ClientVersion.FromFiveBytes(new Span<byte>(data, 5)); } }
      set { unsafe { fixed (byte* data = this.version) value.CopyToFiveBytes(new Span<byte>(data, 5)); } }
    }
  }
}
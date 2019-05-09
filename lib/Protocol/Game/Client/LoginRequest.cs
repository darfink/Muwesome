using System;
using System.Runtime.InteropServices;
using System.Text;
using Muwesome.Packet;
using Muwesome.Packet.Utility;
using Muwesome.Protocol.Utility;

#pragma warning disable SA1202
#pragma warning disable SA1501
#pragma warning disable SA1503

namespace Muwesome.Protocol.Game.Client {
  [Packet(0xC1, 0xF1, 0x01)]
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct LoginRequest : IFixedPacket {
    // TODO: Put this somewhere general
    private static readonly byte[] XorCipher = new byte[] { 0xFC, 0xCF, 0xAB };

    private unsafe fixed byte username[10];
    private unsafe fixed byte password[10];
    public LittleEndian<uint> TickCount;
    private unsafe fixed byte version[5];
    private unsafe fixed byte serial[16];

    public string Username {
      get { unsafe { fixed (byte* data = this.username) return PointerHelper.GetString(data, 10, Encoding.ASCII, XorCipher); } }
      set { unsafe { fixed (byte* data = this.username) PointerHelper.SetString(data, 10, value, Encoding.ASCII, XorCipher); } }
    }

    public string Password {
      get { unsafe { fixed (byte* data = this.password) return PointerHelper.GetString(data, 10, Encoding.ASCII, XorCipher); } }
      set { unsafe { fixed (byte* data = this.password) PointerHelper.SetString(data, 10, value, Encoding.ASCII, XorCipher); } }
    }

    public ClientVersion Version {
      get { unsafe { fixed (byte* data = this.version) return ClientVersion.FromFiveBytes(new Span<byte>(data, 5)); } }
      set { unsafe { fixed (byte* data = this.version) value.CopyToFiveBytes(new Span<byte>(data, 5)); } }
    }

    public Span<byte> Serial {
      get { unsafe { fixed (byte* data = this.serial) return new Span<byte>(data, 16); } }
      set { unsafe { fixed (byte* data = this.serial) value.CopyTo(new Span<byte>(data, 16)); } }
    }
  }
}
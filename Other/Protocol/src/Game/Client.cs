using System;
using System.Text;
using System.Runtime.InteropServices;
using Muwesome.Packet;
using Muwesome.Packet.Utility;
using Muwesome.Protocol.Utility;

namespace Muwesome.Protocol.Game {
  [Packet(0xC1, 0x0E, 0x00)]
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct ClientTime : IFixedPacket {
    public LittleEndian<uint> TickCount;
    public LittleEndian<ushort> AttackSpeed;
    public LittleEndian<ushort> MagicSpeed;
  }

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
      get { unsafe { fixed (byte* data = username) return PointerHelper.GetString(data, 10, Encoding.ASCII, XorCipher); } }
      set { unsafe { fixed (byte* data = username) PointerHelper.SetString(data, 10, value, Encoding.ASCII, XorCipher); } }
    }

    public string Password {
      get { unsafe { fixed (byte* data = password) return PointerHelper.GetString(data, 10, Encoding.ASCII, XorCipher); } }
      set { unsafe { fixed (byte* data = password) PointerHelper.SetString(data, 10, value, Encoding.ASCII, XorCipher); } }
    }

    public ClientVersion Version {
      get { unsafe { fixed (byte* data = version) return ClientVersion.FromFiveBytes(new Span<byte>(data, 5)); } }
      set { unsafe { fixed (byte* data = version) value.CopyToFiveBytes(new Span<byte>(data, 5)); } }
    }

    public Span<byte> Serial {
      get { unsafe { fixed (byte* data = serial) return new Span<byte>(data, 16); } }
      set { unsafe { fixed (byte* data = serial) value.CopyTo(new Span<byte>(data, 16)); } }
    }
  }
}
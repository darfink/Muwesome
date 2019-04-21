using System;
using System.Text;
using Muwesome.Packet;

using System.Runtime.InteropServices;

namespace Muwesome.Protocol.Connect.V20050502 {
  [Packet(0xC1, 0x00, Size = 4)]
  public sealed class ConnectResult : IFixedSizedPacket {
    public static Payload From(Span<byte> packet) =>
      new Payload(PacketFor<ConnectResult>.AsPayload(packet));

    public ref struct Payload {
      private readonly Span<byte> _data;

      public Payload(Span<byte> data) => _data = data;

      public bool Success {
        get => _data.ReadByte() == 1;
        set => _data.WriteByte((byte)(value ? 1 : 0));
      }
    }
  }

  [Packet(0xC1, 0xF4, 0x03, Size = 22)]
  public sealed class GameServerInfo : IFixedSizedPacket {
    public static Payload From(Span<byte> packet, Encoding encoding) =>
      new Payload(PacketFor<GameServerInfo>.AsPayload(packet), encoding);

    public ref struct Payload {
      private readonly Span<byte> _data;
      private Encoding _encoding;

      public Payload(Span<byte> data, Encoding encoding) {
        _encoding = encoding;
        _data = data;
      }

      public string Host {
        get => _data.ReadString(maxLength: 16, _encoding);
        set => _data.WriteFixedString(value, length: 16, withNull: true, _encoding);
      }

      public ushort Port {
        get => _data.ReadUInt16LE(16);
        set => _data.WriteUInt16LE(value, 16);
      }
    }
  }

  [Packet(0xC1, 0xF4, 0x05, Size = 6)]
  public sealed class GameServerUnavailable : IFixedSizedPacket {
    public static Payload From(Span<byte> packet) =>
      new Payload(PacketFor<GameServerUnavailable>.AsPayload(packet));

    public ref struct Payload {
      private readonly Span<byte> _data;

      public Payload(Span<byte> data) => _data = data;

      public ushort ServerCode {
        get => _data.ReadUInt16LE();
        set => _data.WriteUInt16LE(value);
      }
    }
  }

  [Packet(0xC2, 0xF4, 0x06)]
  public sealed class GameServerList : IVariableSizedPacket {
    public static Payload View(Span<byte> packet) => new Payload(packet);

    public ref struct Payload {
      private readonly Span<byte> _data;

      public Payload(Span<byte> data) => _data = data;

      public ushort Length => _data.ReadUInt16LE();

      public Enumerator GetEnumerator() {
        return new Enumerator { };
      }

      public ref struct Enumerator {
        private Span<byte> _data;

        private int index;
        public Entry Current => new Entry(_data.Slice(index * 4));
        public bool MoveNext() => true;
      }

      public ref struct Entry {
        public static int Size = 4;

        private Span<byte> _data;
        public Entry(Span<byte> data) => _data = data;
      }
    }
  }
}
using System;
using System.Text;
using Muwesome.Packet;

using System.Runtime.InteropServices;

namespace Muwesome.Protocol.Connect.V20050502 {
  public struct GameServerCode {
  }

  [Packet(0xC1, 0x00, Size = 4)]
  public sealed class ConnectResult : IFixedSizedPacket {
    public static Payload From(Span<byte> packet) =>
      new Payload(PacketFor<ConnectResult>.AsPayload(packet));

    public ref struct Payload {
      private readonly Span<byte> _data;

      /// <summary>Constructs a <see cref="ConnectResult" /> payload.</summary>
      public Payload(Span<byte> data) => _data = data;

      /// <summary>Gets or sets the success result.</summary>
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

      /// <summary>Constructs a <see cref="GameServerInfo" /> payload.</summary>
      public Payload(Span<byte> data, Encoding encoding) {
        _encoding = encoding;
        _data = data;
      }

      /// <summary>Gets or sets the server's host.</summary>
      /// <remarks>
      /// The client performs a DNS lookup in case the host field is not a valid
      /// IP address, which allows domains to be used as well.
      /// </remarks>
      public string Host {
        get => _data.ReadString(maxLength: 16, _encoding);
        set => _data.WriteFixedString(value, length: 16, withNull: true, _encoding);
      }

      /// <summary>Gets or sets the server's port.</summary>
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

      /// <summary>Constructs a <see cref="GameServerUnavailable" /> payload.</summary>
      public Payload(Span<byte> data) => _data = data;

      /// <summary>Gets or sets the server's code.</summary>
      public ushort Code {
        get => _data.ReadUInt16LE();
        set => _data.WriteUInt16LE(value);
      }
    }
  }

  [Packet(0xC2, 0xF4, 0x06)]
  public sealed class GameServerList : IVariableSizedPacket {
    public static Payload From(Span<byte> packet) =>
      new Payload(PacketFor<GameServerList>.AsPayload(packet));

    public ref struct Payload {
      private readonly Span<byte> _data;

      /// <summary>Constructs a <see cref="GameServerList" /> payload.</summary>
      public Payload(Span<byte> data) => _data = data;

      /// <summary>Gets or sets the number of server's.</summary>
      public ushort Length {
        get => _data.ReadUInt16LE();
        set {
          // TODO: Validate input
          _data.WriteUInt16LE(value);
        }
      }

      /// <summary>Gets a server entry.</summary>
      public Entry this[int index] => new Entry(_data.Slice(2 + index * 4));

      /// <summary>Gets an enumerator over all server entries.</summary>
      public Enumerator GetEnumerator() => new Enumerator(this);

      public ref struct Entry {
        private Span<byte> _data;

        public Entry(Span<byte> data) => _data = data;
      }

      public ref struct Enumerator {
        private Payload _payload;
        private int _index;

        public Enumerator(Payload payload) {
          _payload = payload;
          _index = -1;
        }

        public Entry Current => _payload[_index];
        public bool MoveNext() => ++_index < _payload.Length;
      }
    }
  }
}
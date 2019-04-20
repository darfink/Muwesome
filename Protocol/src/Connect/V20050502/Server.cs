using System;
using Muwesome.Packet;

using System.Runtime.InteropServices;

namespace Muwesome.Protocol.Connect.V20050502 {
  [Packet(0xC2, 0xF4, 0x06)]
  public class GameServerList : PacketDefinition {
    public static Packet View(Span<byte> packet) => new Packet(packet);

    public ref struct Packet {
      private readonly Span<byte> _data;

      public Packet(Span<byte> data) => _data = data;

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
        private Span<byte> _data;
        public Entry(Span<byte> data) => _data = data;
      }
    }
  }

  [Packet(0xC1, 0xF4, 0x05)]
  public class GameServerUnavailable : PacketDefinition {
    public static Packet Cast(Span<byte> packet) => new Packet(packet.Slice(4));

    public ref struct Packet {
      private readonly Span<byte> _data;

      public ushort ServerCode {
        get => _data.ReadUInt16LE();
        set => _data.WriteUInt16LE(value);
      }
    }
  }
}
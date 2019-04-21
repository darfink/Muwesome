using System;
using System.Collections.Generic;
using System.Threading;
using Muwesome.Packet;

namespace Muwesome.Network {
  public struct ThreadSafeWriter : IDisposable {
    private IConnection _connection;
    private Memory<byte> _memory;
    private int _packetSize;

    /// <summary>Creates a new thread safe writer transaction.</summary>
    public static ThreadSafeWriter CreateWith<TPacket>(IConnection connection) where TPacket : IFixedSizedPacket =>
      new ThreadSafeWriter(connection, PacketFor<TPacket>.Size.Value).WritePacketHeader<TPacket>();

    /// <summary>Creates a new thread safe writer transaction.</summary>
    public static ThreadSafeWriter CreateWith<TPacket>(IConnection connection, int packetSize) where TPacket : IVariableSizedPacket =>
      new ThreadSafeWriter(connection, packetSize).WritePacketHeader<TPacket>();

    /// <summary>Creates a new <see cref="ThreadSafeWriter" />.</summary>
    private ThreadSafeWriter(IConnection connection, int packetSize) {
      Monitor.Enter(connection);
      _connection = connection;
      _packetSize = packetSize;
      _memory = connection.Output.GetMemory(_packetSize).Slice(_packetSize);
    }

    /// <summary>Gets the span for the current transaction.</summary>
    public Span<byte> Span => _memory.Span;

    /// <summary>Commits all changes to the underlying connection.</summary>
    public void Dispose() {
      try {
        _connection.Output.Advance(_packetSize);
        _connection.Output.FlushAsync();
      } finally {
        Monitor.Exit(_connection);
      }
    }

    private ThreadSafeWriter WritePacketHeader<TPacket>() where TPacket : IPacket {
      var span = this.Span;
      span.Clear();
      PacketFor<TPacket>.CopyTo(span, _packetSize);
      return this;
    }
  }
}
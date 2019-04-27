using System;
using System.Collections.Generic;
using System.Threading;
using Muwesome.Packet;

namespace Muwesome.Network {
  public struct ThreadSafeWriter : IDisposable {
    private IConnection _connection;
    private int _payloadSize;

    /// <summary>Creates a new <see cref="ThreadSafeWriter" />.</summary>
    public ThreadSafeWriter(IConnection connection, int payloadSize) {
      Monitor.Enter(connection);
      _connection = connection;
      _payloadSize = payloadSize;
      Span.Clear();
    }

    /// <summary>Gets the span for the current transaction.</summary>
    public Span<byte> Span => _connection.Output.GetSpan(_payloadSize).Slice(0, _payloadSize);

    /// <summary>Commits all changes to the underlying connection.</summary>
    public void Dispose() {
      Console.WriteLine("Sent: " + Span.AsHexString());
      try {
        // TODO: Improve/move this logic
        new PacketView(Span).ValidateHeader();
        _connection.Output.Advance(_payloadSize);
        _connection.Output.FlushAsync();
      } finally {
        Monitor.Exit(_connection);
      }
    }
  }
}
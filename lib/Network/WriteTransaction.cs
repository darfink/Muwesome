using System;
using System.Collections.Generic;
using System.Threading;
using Muwesome.Packet;
using Muwesome.Packet.Utility;

namespace Muwesome.Network {
  public struct WriteTransaction : IDisposable {
    private readonly IConnection connection;
    private readonly int payloadSize;

    /// <summary>Initializes a new instance of the <see cref="WriteTransaction"/> struct.</summary>
    internal WriteTransaction(IConnection connection, int payloadSize) {
      Monitor.Enter(connection);
      this.connection = connection;
      this.payloadSize = payloadSize;
      this.Span.Clear();
    }

    /// <summary>Gets the span for the current transaction.</summary>
    public Span<byte> Span => this.connection.Output.GetSpan(this.payloadSize).Slice(0, this.payloadSize);

    /// <summary>Commits all changes to the underlying connection.</summary>
    public void Dispose() {
      try {
        // TODO: Improve/move this logic
        new PacketView(this.Span).ValidateHeader();
        this.connection.Output.Advance(this.payloadSize);
        this.connection.Output.FlushAsync();
      } finally {
        Monitor.Exit(this.connection);
      }
    }
  }
}
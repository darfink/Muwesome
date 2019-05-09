using System;
using System.IO.Pipelines;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Muwesome.Packet.IO;

namespace Muwesome.Network {
  public class DuplexConnection : IConnection {
    private readonly IDuplexPipe duplexPipe;
    private readonly PacketEventReader packetReader;
    private int isDisposed = 0;

    /// <summary>Initializes a new instance of the <see cref="DuplexConnection"/> class.</summary>
    public DuplexConnection(IDuplexPipe pipe, int? maxPacketSize = null) {
      this.duplexPipe = pipe;
      this.packetReader = new PacketEventReader(pipe.Input, maxPacketSize);
    }

    /// <inheritdoc />
    public event PacketReceivedHandler PacketReceived;

    /// <inheritdoc />
    public event DisconnectedHandler Disconnected;

    /// <inheritdoc />
    public PipeWriter Output => this.duplexPipe.Output;

    /// <inheritdoc />
    public EndPoint RemoteEndPoint => this.RemotePipe?.RemoteEndPoint;

    /// <inheritdoc />
    public bool IsConnected => this.isDisposed == 0 && (this.RemotePipe?.IsBound ?? true);

    /// <summary>Gets the pipe as a <see cref="IRemoteDuplexPipe" /> if possible.</summary>
    private IRemoteDuplexPipe RemotePipe => this.duplexPipe as IRemoteDuplexPipe;

    /// <inheritdoc />
    public async Task BeginReceive() {
      if (this.isDisposed == 1) {
        throw new ObjectDisposedException(nameof(DuplexConnection));
      }

      try {
        await this.packetReader.BeginRead(packet => this.PacketReceived?.Invoke(this, packet));
      } finally {
        this.Disconnect();
      }
    }

    /// <inheritdoc />
    public void Disconnect() => this.Dispose();

    /// <inheritdoc />
    public void Dispose() {
      if (Interlocked.Exchange(ref this.isDisposed, 1) == 1) {
        return;
      }

      (this.duplexPipe as IDisposable)?.Dispose();
      this.Disconnected?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc/>
    public override string ToString() => this.duplexPipe.ToString();
  }
}
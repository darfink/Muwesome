using System;
using System.IO.Pipelines;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Muwesome.Packet;
using Muwesome.Packet.IO;

namespace Muwesome.Network {
  public class DuplexConnection : IConnection {
    private readonly IDuplexPipe _duplexPipe;
    private readonly PacketEventReader _packetReader;
    private int _isDisposed = 0;

    /// <summary>Constructs a new connection instance.</summary>
    public DuplexConnection(IDuplexPipe pipe, int? maxPacketSize = null) {
      _duplexPipe = pipe;
      _packetReader = new PacketEventReader(pipe.Input, maxPacketSize);
    }

    /// <inheritdoc />
    public event PacketReceivedHandler PacketReceived;

    /// <inheritdoc />
    public event DisconnectedHandler Disconnected;

    /// <inheritdoc />
    public PipeWriter Output => _duplexPipe.Output;

    /// <inheritdoc />
    public EndPoint RemoteEndPoint => RemotePipe?.RemoteEndPoint;

    /// <inheritdoc />
    public bool IsConnected => _isDisposed == 0 && (RemotePipe?.IsBound ?? true);

    /// <summary>Gets the pipe as a <see cref="IRemoteDuplexPipe" /> if possible.</summary>
    private IRemoteDuplexPipe RemotePipe => _duplexPipe as IRemoteDuplexPipe;

    /// <inheritdoc />
    public async Task BeginReceive() {
      if (_isDisposed == 1) {
        throw new ObjectDisposedException(nameof(DuplexConnection));
      }

      try {
        await _packetReader.BeginRead(packet => PacketReceived?.Invoke(this, packet));
      } finally {
        Disconnect();
      }
    }

    /// <inheritdoc />
    public void Disconnect() => Dispose();

    /// <inheritdoc />
    public void Dispose() {
      if (Interlocked.Exchange(ref _isDisposed, 1) == 1) {
        return;
      }

      (_duplexPipe as IDisposable)?.Dispose();
      Disconnected?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc/>
    public override string ToString() => _duplexPipe.ToString();
  }
}

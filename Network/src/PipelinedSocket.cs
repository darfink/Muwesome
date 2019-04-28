using System;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using Muwesome.Packet.IO;
using Pipelines.Sockets.Unofficial;

namespace Muwesome.Network {
  public class PipelinedSocket : IRemoteDuplexPipe, IDisposable {
    private readonly IPipelineEncryptor encryptor;
    private readonly IPipelineDecryptor decryptor;
    private readonly SocketConnection socket;
    private bool pipesActive = true;

    /// <summary>Initializes a new instance of the <see cref="PipelinedSocket"/> class.</summary>
    public PipelinedSocket(SocketConnection socket, IPipelineEncryptor encryptor, IPipelineDecryptor decryptor) {
      this.encryptor = encryptor;
      this.decryptor = decryptor;
      this.socket = socket;
      this.socket.Input.OnWriterCompleted((_, ev) => this.pipesActive = false, null);
      this.socket.Output.OnReaderCompleted((_, ev) => this.pipesActive = false, null);
      this.RemoteEndPoint = this.socket.Socket.RemoteEndPoint;
    }

    /// <inheritdoc />
    public bool IsBound => this.pipesActive && this.socket.Socket.Connected;

    /// <inheritdoc />
    public EndPoint RemoteEndPoint { get; }

    /// <inheritdoc />
    public PipeReader Input => this.decryptor?.Reader ?? this.socket.Input;

    /// <inheritdoc />
    public PipeWriter Output => this.encryptor?.Writer ?? this.socket.Output;

    /// <inheritdoc />
    public void Dispose() => this.socket.Dispose();

    /// <inheritdoc/>
    public override string ToString() => this.RemoteEndPoint?.ToString() ?? $"<unknown>";
  }
}
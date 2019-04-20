using System;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using Muwesome.Packet.IO;
using Pipelines.Sockets.Unofficial;

namespace Muwesome.Network {
  public class PipelinedSocket : IRemoteDuplexPipe, IDisposable {
    private readonly IPipelineEncryptor _encryptor;
    private readonly IPipelineDecryptor _decryptor;
    private readonly SocketConnection _socket;
    private bool _pipesActive = true;

    /// <summary>Constructs a pipelined socket.</summary>
    public PipelinedSocket(SocketConnection socket, IPipelineEncryptor encryptor, IPipelineDecryptor decryptor) {
      _encryptor = encryptor;
      _decryptor = decryptor;
      _socket = socket;
      _socket.Input.OnWriterCompleted((_, __) => _pipesActive = false, null);
      _socket.Output.OnReaderCompleted((_, __) => _pipesActive = false, null);
      RemoteEndPoint = _socket.Socket.RemoteEndPoint;
    }

    /// <inheritdoc />
    public bool IsBound => _pipesActive && _socket.Socket.Connected;

    /// <inheritdoc />
    public EndPoint RemoteEndPoint { get; }

    /// <inheritdoc />
    public PipeReader Input => _decryptor?.Reader ?? _socket.Input;

    /// <inheritdoc />
    public PipeWriter Output => _encryptor?.Writer ?? _socket.Output;

    /// <inheritdoc />
    public void Dispose() => _socket.Dispose();

    /// <inheritdoc/>
    public override string ToString() =>
      _socket.Socket.RemoteEndPoint?.ToString() ?? $"<unknown>";
  }
}
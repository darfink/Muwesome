using System;
using System.Net;
using System.Threading;
using log4net;
using Muwesome.Network;
using Muwesome.Packet;
using Muwesome.Protocol;

namespace Muwesome.ConnectServer {
  public class Client : IDisposable {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(Client));
    private readonly IPacketHandler<Client> _packetHandler;
    private TimeSpan _maxIdleTime = Timeout.InfiniteTimeSpan;
    private Timer _idleTimeoutTimer;
    private int _isDisposed = 0;

    /// <summary>Constructs a new client instance.</summary>
    public Client(IConnection connection, IPacketHandler<Client> packetHandler) {
      _packetHandler = packetHandler;
      Connection = connection;
      Connection.PacketReceived += OnPacketReceived;
      Connection.BeginReceive().ContinueWith(task => OnReceiveComplete(task.Exception));
    }

    /// <summary>Gets the client's connection.</summary>
    public IConnection Connection { get; }

    /// <summary>Gets or sets the maximum idle time.</summary>
    internal TimeSpan MaxIdleTime {
      get => _maxIdleTime;
      set {
        _maxIdleTime = value;
        if (_idleTimeoutTimer == null) {
          _idleTimeoutTimer = new Timer(OnClientTimeout, null, value, Timeout.InfiniteTimeSpan);
        } else {
          _idleTimeoutTimer.Change(value, Timeout.InfiniteTimeSpan);
        }
      }
    }

    /// <inheritdoc />
    public void Dispose() {
      if (Interlocked.Exchange(ref _isDisposed, 1) == 1) {
        return;
      }

      _idleTimeoutTimer?.Dispose();
      Connection.PacketReceived -= OnPacketReceived;
      Connection.Dispose();
    }

    /// <inheritdoc />
    public override string ToString() => Connection.ToString();

    private void OnClientTimeout(object context) {
      Logger.Info($"Disconnecting client {this}; max idle time ({_maxIdleTime}) exceeded");
      Connection.Disconnect();
    }

    private void OnPacketReceived(object sender, Span<byte> packet) {
      _idleTimeoutTimer?.Change(_maxIdleTime, Timeout.InfiniteTimeSpan);
      _packetHandler.HandlePacket(this, packet);
    }

    private void OnReceiveComplete(Exception ex) {
      if (ex != null) {
        Logger.Error("An unexpected client error occured", ex);
      }
    }
  }
}
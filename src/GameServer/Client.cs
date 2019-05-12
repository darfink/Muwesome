using System;
using System.Net;
using System.Threading;
using log4net;
using Muwesome.Common.Utility;
using Muwesome.GameLogic;
using Muwesome.GameServer.Protocol;
using Muwesome.Network;
using Muwesome.Packet;
using Muwesome.Protocol;
using Muwesome.Protocol.Game;

namespace Muwesome.GameServer {
  /// <summary>Represents a connected client.</summary>
  public class Client : IDisposable {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(Client));
    private readonly IPacketHandler<Client> packetHandler;
    private TimeSpan maxIdleTime = Timeout.InfiniteTimeSpan;
    private Timer idleTimeoutTimer;
    private int isDisposed = 0;

    /// <summary>Initializes a new instance of the <see cref="Client"/> class.</summary>
    internal Client(
        IConnection connection,
        ClientProtocol protocol) {
      this.packetHandler = protocol.PacketHandler;
      this.PacketDispatcher = protocol.PacketDispatcher;
      this.Connection = connection;
      this.Connection.PacketReceived += this.OnPacketReceived;
      this.Connection.BeginReceive().ContinueWith(task => this.OnReceiveComplete(task.Exception));
      this.Player = new Player();
    }

    /// <summary>Gets the client's connection.</summary>
    public IConnection Connection { get; }

    /// <summary>Gets the client's player instance.</summary>
    public Player Player { get; private set; }

    /// <summary>Gets or sets the client's version.</summary>
    public ClientVersion Version { get; set; }

    /// <summary>Gets or sets the client's serial.</summary>
    public byte[] Serial { get; set; }

    /// <summary>Gets the client's packet dispatcher.</summary>
    internal ClientPacketDispatcher PacketDispatcher { get; private set; }

    /// <summary>Gets or sets the maximum idle time.</summary>
    internal TimeSpan MaxIdleTime {
      get => this.maxIdleTime;
      set {
        this.maxIdleTime = value;
        if (this.idleTimeoutTimer == null) {
          this.idleTimeoutTimer = new Timer(this.OnClientTimeout, null, value, Timeout.InfiniteTimeSpan);
        } else {
          this.idleTimeoutTimer.Change(value, Timeout.InfiniteTimeSpan);
        }
      }
    }

    /// <inheritdoc />
    public void Dispose() {
      if (Interlocked.Exchange(ref this.isDisposed, 1) == 1) {
        return;
      }

      this.idleTimeoutTimer?.Dispose();
      this.Connection.PacketReceived -= this.OnPacketReceived;
      this.Connection.Dispose();
      this.Player.Dispose();
    }

    /// <inheritdoc />
    public override string ToString() => this.Connection.ToString();

    private void OnClientTimeout(object sender) {
      Logger.Info($"Disconnecting client {this}; max idle time ({this.maxIdleTime}) exceeded");
      this.Connection.Disconnect();
    }

    private void OnPacketReceived(object sender, Span<byte> packet) {
      this.idleTimeoutTimer?.Change(this.maxIdleTime, Timeout.InfiniteTimeSpan);
      this.packetHandler.HandlePacket(this, packet);
    }

    private void OnReceiveComplete(Exception ex) {
      if (ex == null) {
        return;
      }

      var socketException = ex.FindExceptionByType<System.Net.Sockets.SocketException>();
      if (socketException?.SocketErrorCode == System.Net.Sockets.SocketError.ConnectionReset) {
        return;
      }

      Logger.Error("A client session error occured", ex);
    }
  }
}
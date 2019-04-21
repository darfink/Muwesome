using System;
using System.IO.Pipelines;
using System.Net;
using System.Threading.Tasks;
using Muwesome.Packet;

namespace Muwesome.Network {
  /// <summary>A delegate which is executed whenever a packet is received.</summary>
  public delegate void PacketReceivedHandler(object sender, Span<byte> packet);

  /// <summary>A delegate which is executed whenever the connection is disconnected.</summary>
  public delegate void DisconnectedHandler(object sender, EventArgs ev);

  public interface IConnection : IDisposable {
    /// <summary>An event that is raised whenever a packet is received.</summary>
    event PacketReceivedHandler PacketReceived;

    /// <summary>An event that is raised whenever the connection is disconnected.</summary>
    event DisconnectedHandler Disconnected;

    /// <summary>A writer for outgoing packets.</summary>
    PipeWriter Output { get; }

    /// <summary>Gets the connection's end point.</summary>
    EndPoint RemoteEndPoint { get; }

    /// <summary>Gets whether the connection is still active.</summary>
    bool IsConnected { get; }

    /// <summary>Begins reading incoming packets.</summary>
    Task BeginReceive();

    /// <summary>Disconnects the connection.</summary>
    void Disconnect();
  }
}
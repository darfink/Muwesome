using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Muwesome.Interfaces {
  /// <summary>A description of a game server.</summary>
  public class GameServerInfo : INotifyPropertyChanged {
    private uint clientCapacity;
    private uint clientCount;

    /// <summary>Initializes a new instance of the <see cref="GameServerInfo"/> class.</summary>
    public GameServerInfo(ushort code, string host, ushort port, uint clientCount, uint clientCapacity) {
      this.Code = code;
      this.Host = host;
      this.Port = port;
      this.clientCount = clientCount;
      this.clientCapacity = clientCapacity;
    }

    /// <inheritdoc />
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>Gets the server's code.</summary>
    public ushort Code { get; }

    /// <summary>Gets the server's host string.</summary>
    public string Host { get; }

    /// <summary>Gets the server's port.</summary>
    public ushort Port { get; }

    /// <summary>Gets or sets the server's client count.</summary>
    public uint ClientCount {
      get => this.clientCount;
      set => this.SetField(ref this.clientCount, value);
    }

    /// <summary>Gets or sets the server's client capacity.</summary>
    public uint ClientCapacity {
      get => this.clientCapacity;
      set => this.SetField(ref this.clientCapacity, value);
    }

    /// <summary>Gets a value indicating whether the server is at full capacity or not.</summary>
    public bool IsFull => this.ClientCount == this.ClientCapacity;

    /// <summary>Gets the server's current load.</summary>
    public float Load => (float)this.ClientCount / this.ClientCapacity;

    /// <inheritdoc />
    public override string ToString() => $"{this.Host}:{this.Port} [{this.Code}]";

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null) {
      if (EqualityComparer<T>.Default.Equals(field, value)) {
        return false;
      }

      field = value;
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      return true;
    }
  }
}
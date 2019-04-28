using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Muwesome.ConnectServer {
  public class GameServer : INotifyPropertyChanged {
    private uint _clientCapacity;
    private uint _clientCount;

    /// <summary>Constructs a new <see cref="GameServer" />.</summary>
    public GameServer(ushort code, string host, ushort port, uint clientCount, uint clientCapacity) {
      Code = code;
      Host = host;
      Port = port;
      _clientCount = clientCount;
      _clientCapacity = clientCapacity;
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
      get => _clientCount;
      set => SetField(ref _clientCount, value);
    }

    /// <summary>Gets or sets the server's client capacity.</summary>
    public uint ClientCapacity {
      get => _clientCapacity;
      set => SetField(ref _clientCapacity, value);
    }

    /// <summary>Gets whether the server is at full capacity or not.</summary>
    public bool IsFull => ClientCount == ClientCapacity;

    /// <summary>Gets the server's current load.</summary>
    public float Load => (float)ClientCount / ClientCapacity;

    /// <inheritdoc />
    public override string ToString() => $"{Host}:{Port}<{Code}>";

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null) {
      if (EqualityComparer<T>.Default.Equals(field, value)) {
        return false;
      }

      field = value;
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      return true;
    }
  }
}
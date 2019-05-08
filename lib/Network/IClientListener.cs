using System;
using Muwesome.Interfaces;

namespace Muwesome.Network {
  public interface IClientListener : ILifecycle {
    /// <summary>An event that is raised after a client has connected.</summary>
    event EventHandler<ClientConnectedEventArgs> ClientConnected;
  }
}
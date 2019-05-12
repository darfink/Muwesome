using System;
using Muwesome.Common;

namespace Muwesome.Network {
  public interface IClientListener<TClient> : ILifecycle {
    /// <summary>An event that is raised after a client connection has been established.</summary>
    event EventHandler<ClientConnectionEstablishedEventArgs> ClientConnectionEstablished;

    /// <summary>An event that is raised after a client has connected.</summary>
    event EventHandler<ClientConnectedEventArgs<TClient>> ClientConnected;
  }
}
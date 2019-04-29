using System;
using Muwesome.ServerCommon;

namespace Muwesome.ConnectServer {
  public interface IClientListener : ILifecycle {
    /// <summary>An event that is raised before a client is accepted.</summary>
    event EventHandler<BeforeClientAcceptEventArgs> BeforeClientAccepted;

    /// <summary>An event that is raised after a client has been accepted.</summary>
    event EventHandler<AfterClientAcceptEventArgs> AfterClientAccepted;

    /// <summary>Gets a value indicating whether the listener is bound or not.</summary>
    bool IsBound { get; }
  }
}
using System;
using System.IO.Pipelines;
using System.Net;

namespace Muwesome.Network {
  public interface IRemoteDuplexPipe : IDuplexPipe {
    /// <summary>Gets the remote's end point.</summary>
    EndPoint RemoteEndPoint { get; }

    /// <summary>Gets a value indicating whether the pipe is still bound.</summary>
    bool IsBound { get; }
  }
}
namespace Muwesome.Network.Tcp.Filters {
  /// <summary>Extensions for client TCP listeners.</summary>
  public static class ClientTcpListenerExtensions {
    /// <summary>Adds a filter to a client TCP listener.</summary>
    public static void AddFilter<T>(this ClientTcpListenerBase<T> clientListener, IClientSocketFilter filter) =>
      filter.Register(clientListener);
  }
}
namespace Muwesome.Network.Tcp.Filters {
  /// <summary>Represents a client socket filter.</summary>
  public interface IClientSocketFilter {
    /// <summary>Registers the filter with a TPC listener.</summary>
    void Register<T>(IClientTcpListener<T> clientListener);
  }
}
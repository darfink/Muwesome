namespace Muwesome.Rpc {
  /// <summary>Represents an RPC end point as a host and a port number.</summary>
  public class RpcEndPoint {
    /// <summary>Gets or sets the RPC host domain or address.</summary>
    public string Host { get; set; }

    /// <summary>Gets or sets the RPC port.</summary>
    public ushort Port { get; set; }

    /// <inheritdoc />
    public override string ToString() => $"{this.Host}:{this.Port}";
  }
}
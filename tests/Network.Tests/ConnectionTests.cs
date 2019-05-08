using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Sockets;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Muwesome.Network;

namespace Muwesome.Network.Tests {
  class DuplexPipe : IDuplexPipe {
    public DuplexPipe() {
      var pipe = new Pipe();
      Input = pipe.Reader;
      Output = pipe.Writer;
    }

    /// <inheritdoc />
    public PipeReader Input { get; private set; }

    /// <inheritdoc />
    public PipeWriter Output { get; private set; }
  }

  [TestClass]
  public class ConnectionTests {
    [TestMethod]
    public async Task Disconnects_When_Receiving_Invalid_Packet() {
      var pipe = new DuplexPipe();
      var cancellation = new CancellationTokenSource();

      using (IConnection connection = new DuplexConnection(pipe)) {
        connection.Disconnected += (_, __) => cancellation.Cancel();
        _ = connection.BeginReceive();

        try {
          await connection.Output.WriteAsync(new byte[] { 0xFF, 0xFF, 0xFF });
          await connection.Output.FlushAsync();
        } catch { }

        await Task.Delay(100, cancellation.Token).ContinueWith(_ => { });
        Assert.IsTrue(cancellation.IsCancellationRequested);
        Assert.IsFalse(connection.IsConnected);
      }
    }
  }
}

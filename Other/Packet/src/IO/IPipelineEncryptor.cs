using System.IO.Pipelines;

namespace Muwesome.Packet.IO {
  public interface IPipelineEncryptor {
    /// <summary>Gets the writer for encrypted packets.</summary>
    PipeWriter Writer { get; }
  }
}
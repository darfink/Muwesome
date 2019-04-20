using System.IO.Pipelines;

namespace Muwesome.Packet.IO {
  public interface IPipelineDecryptor {
    /// <summary>Gets the reader for decrypted packets.</summary>
    PipeReader Reader { get; }
  }
}
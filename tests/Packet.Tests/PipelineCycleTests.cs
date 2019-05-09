using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Muwesome.Packet.IO;
using Muwesome.Packet.IO.Xor;

namespace Muwesome.Packet.Tests {
  [TestClass]
  public class PipelineCycleTests {
    [TestMethod]
    public async Task Encrypt_Decrypt_C1_Xor_Cycle() {
      await this.EncryptDecryptWithXor(Samples.UnencryptedC1Packet);
    }

    [TestMethod]
    public async Task Encrypt_Decrypt_C2_Xor_Cycle() {
      await this.EncryptDecryptWithXor(Samples.UnencryptedC2Packet);
    }

    private async Task EncryptDecryptWithXor(byte[] unencryptedPacket) {
      var pipe = new Pipe();
      var encryptor = new XorPipelineEncryptor(pipe.Writer);
      var decryptor = new XorPipelineDecryptor(pipe.Reader);

      await encryptor.Writer.WriteAsync(unencryptedPacket);
      await encryptor.Writer.FlushAsync();

      var result = await decryptor.Reader.ReadAsync();
      CollectionAssert.AreEqual(result.Buffer.ToArray(), unencryptedPacket);
    }
  }
}
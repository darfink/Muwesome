using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Muwesome.Packet.IO;
using Muwesome.Packet.IO.SimpleModulus;
using Muwesome.Packet.IO.Xor;
using Muwesome.Packet.Utility;

namespace Muwesome.Packet.Tests {
  [TestClass]
  public class PipelineDecryptorTests {
    [TestMethod]
    public async Task Decrypts_C1_With_Xor_Cipher() {
      await this.DecryptWithXor(Samples.XorEncryptedC1Packet, Samples.UnencryptedC1Packet);
    }

    [TestMethod]
    public async Task Decrypts_C2_With_Xor_Cipher() {
      await this.DecryptWithXor(Samples.XorEncryptedC2Packet, Samples.UnencryptedC2Packet);
    }

    [TestMethod]
    public async Task Rejects_Decryption_Of_Non_Packet() {
      await Assert.ThrowsExceptionAsync<InvalidPacketTypeException>(
        async () => await this.DecryptWithXor(new byte[] { 5, 9, 25 }, Array.Empty<byte>()));
    }

    [TestMethod]
    public async Task Rejects_Decryption_Of_Invalid_Packet_Size() {
      await Assert.ThrowsExceptionAsync<InvalidPacketSizeException>(
        async () => await this.DecryptWithXor(Samples.InvalidSizePacket, Array.Empty<byte>()));
    }

    [TestMethod]
    public async Task Handles_Partial_Packet_Writes() {
      var pipe = new Pipe();
      var decryptor = new UniformPipelineReader(pipe.Reader);

      pipe.Writer.Write(new Span<byte>(Samples.UnencryptedC1Packet, 0, 3));
      await pipe.Writer.FlushAsync();

      var read = decryptor.Reader.ReadAsync();
      await Task.Delay(50);
      Assert.IsFalse(read.IsCompleted);

      pipe.Writer.Write(new Span<byte>(Samples.UnencryptedC1Packet, 3, Samples.UnencryptedC1Packet.Length - 3));
      await pipe.Writer.FlushAsync();

      await Task.Delay(50);
      Assert.IsTrue(read.IsCompleted);
    }

    [TestMethod]
    public async Task Completes_With_Last_Read() {
      var pipe = new Pipe();
      var decryptor = new UniformPipelineReader(pipe.Reader);

      pipe.Writer.Write(Samples.MinimalPacket);
      pipe.Writer.Complete();

      await Task.Delay(100);
      var result = await decryptor.Reader.ReadAsync();

      Assert.IsTrue(result.IsCompleted);
    }

    [TestMethod]
    public async Task Completes_After_Read_Async_Is_Called() {
      var pipe = new Pipe();
      var decryptor = new UniformPipelineReader(pipe.Reader);

      bool completed = false;
      decryptor.Reader.OnWriterCompleted((e, o) => completed = true, null);

      var reading = decryptor.Reader.ReadAsync();
      pipe.Writer.Complete();
      var result = await reading;

      await Task.Delay(10);
      Assert.IsTrue(completed && result.IsCompleted);
    }

    [TestMethod]
    public async Task Packet_Decryptor_Receives_Packets() {
      const int PacketsToSend = 5;

      var pipe = new Pipe();
      var decryptor = new PacketEventReader(new XorPipelineDecryptor(pipe.Reader).Reader);

      int packetsReceived = 0;
      var readTask = decryptor.BeginRead(packet => packetsReceived += 1);

      for (int i = 0; i < PacketsToSend; i++) {
        await pipe.Writer.WriteAsync(Samples.UnencryptedC1Packet);
        await pipe.Writer.FlushAsync();
      }

      pipe.Writer.Complete();
      await readTask;
      Assert.AreEqual(packetsReceived, PacketsToSend);
    }

    private async Task DecryptWithXor(byte[] encryptedPacket, byte[] decryptedPacket) {
      var pipe = new Pipe();
      var decryptor = new XorPipelineDecryptor(new SimpleModulusPipelineDecryptor(pipe.Reader).Reader);
      await pipe.Writer.WriteAsync(encryptedPacket);
      await pipe.Writer.FlushAsync();
      var result = await decryptor.Reader.ReadAsync();
      CollectionAssert.AreEqual(result.Buffer.ToArray(), decryptedPacket);
    }
  }
}
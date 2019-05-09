using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Muwesome.Packet.IO;
using Muwesome.Packet.IO.Xor;

namespace Muwesome.Packet.Tests {
  [TestClass]
  public class PipelineEncryptorTests {
    [TestMethod]
    public async Task Encrypts_C1_With_Xor_Cipher() {
      await this.EncryptWithXor(Samples.UnencryptedC1Packet, Samples.XorEncryptedC1Packet);
    }

    [TestMethod]
    public async Task Encrypts_C2_With_Xor_Cipher() {
      await this.EncryptWithXor(Samples.UnencryptedC2Packet, Samples.XorEncryptedC2Packet);
    }

    [TestMethod]
    public async Task Rejects_Encryption_Of_Non_Packet() {
      await Assert.ThrowsExceptionAsync<InvalidPacketTypeException>(
        async () => await this.EncryptWithXor(new byte[] { 5, 9, 25 }, new byte[0]));
    }

    [TestMethod]
    public async Task Rejects_Encryption_Of_Invalid_Packet_Size() {
      await Assert.ThrowsExceptionAsync<InvalidPacketSizeException>(
        async () => await this.EncryptWithXor(Samples.InvalidSizePacket, new byte[0]));
    }

    [TestMethod]
    public async Task Handles_Partial_Packet_Writes() {
      var pipe = new Pipe();
      var encryptor = new UniformPipelineWriter(pipe.Writer);

      encryptor.Writer.Write(new Span<byte>(Samples.UnencryptedC1Packet, 0, 3));
      await encryptor.Writer.FlushAsync();

      var read = pipe.Reader.ReadAsync();
      await Task.Delay(50);
      Assert.IsFalse(read.IsCompleted);

      encryptor.Writer.Write(new Span<byte>(Samples.UnencryptedC1Packet, 3, Samples.UnencryptedC1Packet.Length - 3));
      await encryptor.Writer.FlushAsync();

      await Task.Delay(50);
      Assert.IsTrue(read.IsCompleted);
    }

    [TestMethod]
    public async Task Completes_With_Last_Read() {
      var pipe = new Pipe();
      var encryptor = new UniformPipelineWriter(pipe.Writer);
      encryptor.Writer.Write(Samples.MinimalPacket);
      encryptor.Writer.Complete();

      await Task.Delay(100);
      var result = await pipe.Reader.ReadAsync();

      Assert.IsTrue(result.IsCompleted);
    }

    [TestMethod]
    public async Task Completes_After_Read_Async_Is_Called() {
      var pipe = new Pipe();
      var encryptor = new UniformPipelineWriter(pipe.Writer);

      bool completed = false;
      pipe.Reader.OnWriterCompleted((e, o) => completed = true, null);

      var reading = pipe.Reader.ReadAsync();
      encryptor.Writer.Complete();
      var result = await reading;

      await Task.Delay(10);
      Assert.IsTrue(completed && result.IsCompleted);
    }

    private async Task EncryptWithXor(byte[] unencryptedPacket, byte[] encryptedPacket) {
      var pipe = new Pipe();
      var encryptor = new XorPipelineEncryptor(pipe.Writer);
      await encryptor.Writer.WriteAsync(unencryptedPacket);
      await encryptor.Writer.FlushAsync();
      var result = await pipe.Reader.ReadAsync();
      CollectionAssert.AreEqual(result.Buffer.ToArray(), encryptedPacket);
    }
  }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Muwesome.Packet.Tests {
  [TestClass]
  public class PacketViewTests {
    [TestMethod]
    public void View_Complete_C1_Packet() {
      PacketView packet = new PacketView(Samples.UnencryptedC1Packet);
      Assert.AreEqual(packet.Length, Samples.UnencryptedC1Packet.Length);
      Assert.AreEqual(packet.HeaderLength, 2);
      Assert.AreEqual(packet.Type, 0xC1);
      Assert.AreEqual(packet.Code.Value, 0xA9);
      Assert.IsFalse(packet.IsEncrypted);
      Assert.IsFalse(packet.IsPartial);
      CollectionAssert.AreEqual(packet.Data.ToArray(), Samples.UnencryptedC1Packet);
    }

    [TestMethod]
    public void View_Partial_C1_Packet() {
      PacketView packet = new PacketView(new Span<byte>(Samples.UnencryptedC1Packet, 0, 3));
      Assert.AreEqual(packet.Length, Samples.UnencryptedC1Packet.Length);
      Assert.AreEqual(packet.Type, 0xC1);
      Assert.AreEqual(packet.Code.Value, 0xA9);
      Assert.IsTrue(packet.IsPartial);
    }

    [TestMethod]
    public void View_Complete_C2_Packet() {
      PacketView packet = new PacketView(Samples.UnencryptedC2Packet);
      Assert.AreEqual(packet.Length, Samples.UnencryptedC2Packet.Length);
      Assert.AreEqual(packet.HeaderLength, 3);
      Assert.AreEqual(packet.Type, 0xC2);
      Assert.AreEqual(packet.Code.Value, 0xF3);
      Assert.IsFalse(packet.IsEncrypted);
      Assert.IsFalse(packet.IsPartial);
      CollectionAssert.AreEqual(packet.Data.ToArray(), Samples.UnencryptedC2Packet);
    }

    [TestMethod]
    public void View_Partial_C2_Packet() {
      PacketView packet = new PacketView(new Span<byte>(Samples.UnencryptedC2Packet, 0, 3));
      Assert.AreEqual(packet.Length, Samples.UnencryptedC2Packet.Length);
      Assert.AreEqual(packet.Type, 0xC2);
      Assert.AreEqual(packet.Code, null);
      Assert.IsTrue(packet.IsPartial);
    }
  }
}

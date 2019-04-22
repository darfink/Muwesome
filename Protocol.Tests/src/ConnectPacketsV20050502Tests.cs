using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Muwesome.Packet;
using Muwesome.Protocol.Connect.V20050502;

namespace Muwesome.Protocol.Tests {
  [TestClass]
  public class ConnectPacketsV20050502Tests {
    static readonly byte[] ConnectResultPacketSuccess = new byte[] { 0xC1, 0x04, 0x00, 0x01 };
    static readonly byte[] GameServerUnavailablePacket = new byte[] { 0xC1, 0x06, 0xF4, 0x05, 0x39, 0x05 };
    static readonly byte[] GameServerInfoPacket = Convert.FromBase64String("wRb0AzE5Mi4xNjguMS4xMDAAAADUBw==");

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Packet_Identifier_Validation_Fails_When_Mismatch() {
      ConnectResult.From(GameServerInfoPacket);
    }

    [TestMethod]
    public void ConnectResult_Serialization() {
      var packet = PacketFor<ConnectResult>.Create();
      var result = ConnectResult.From(packet);
      result.Success = true;
      CollectionAssert.AreEqual(packet, ConnectResultPacketSuccess);

      result = ConnectResult.From(ConnectResultPacketSuccess);
      Assert.IsTrue(result.Success);
    }

    [TestMethod]
    public void GameServerInfo_Serialization() {
      var packet = PacketFor<GameServerInfo>.Create();
      var server = GameServerInfo.From(packet, Encoding.ASCII);
      server.Host = "192.168.1.100";
      server.Port = 2004;
      CollectionAssert.AreEqual(packet, GameServerInfoPacket);

      server = GameServerInfo.From(GameServerInfoPacket, Encoding.ASCII);
      Assert.AreEqual(server.Host, "192.168.1.100");
      Assert.AreEqual(server.Port, 2004);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GameServerInfo_Serialize_Fails_When_MaxLength_Is_Exceeded() {
      var server = GameServerInfo.From(PacketFor<GameServerInfo>.Create(), Encoding.ASCII);
      server.Host = "192.168.255.255-";
    }

    [TestMethod]
    public void GameServerUnavailable_Serialization() {
      var packet = PacketFor<GameServerUnavailable>.Create();
      var server = GameServerUnavailable.From(packet);
      server.Code = 1337;
      CollectionAssert.AreEqual(packet, GameServerUnavailablePacket);

      server = GameServerUnavailable.From(GameServerUnavailablePacket);
      Assert.AreEqual(server.Code, 1337);
    }
  }
}

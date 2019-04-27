using System;
using System.Text;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Muwesome.Packet;
using Muwesome.Protocol.Connect.V20050502;

namespace Muwesome.Protocol.Tests {
  [TestClass]
  public class ConnectPacketsV20050502Tests {
    static readonly byte[] ConnectResultPacketSuccess = new byte[] { 0xC1, 0x04, 0x00, 0x01 };
    static readonly byte[] GameServerUnavailablePacket = new byte[] { 0xC1, 0x06, 0xF4, 0x05, 0x39, 0x05 };
    static readonly byte[] GameServerInfoPacket = Convert.FromBase64String("wRb0AzE5Mi4xNjguMS4xMDAAAADUBw==");
    static readonly byte[] GameServerListPacket = Convert.FromBase64String("wgAX9AYABAEAIAACAAAAAwAgAAQAgAA=");
    static readonly (ushort, float, bool)[] GameServerListEntries = new (ushort, float, bool)[] { (1, 0.32f, false), (2, 0f, false), (3, 0.32f, false), (4, 0f, true) };

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Packet_Identifier_Validation_Fails_When_Mismatch() {
      ProtocolHelper.ParsePacket<GameServerUnavailable>(GameServerInfoPacket);
    }

    [TestMethod]
    public void ConnectResult_Serialization() {
      ref var result = ref ProtocolHelper.CreatePacket<ConnectResult>(out byte[] data);
      result.Success = true;
      CollectionAssert.AreEqual(ConnectResultPacketSuccess, data);

      result = ProtocolHelper.ParsePacket<ConnectResult>(ConnectResultPacketSuccess);
      Assert.IsTrue(result.Success);
    }

    [TestMethod]
    public void GameServerInfo_Serialization() {
      ref var serverInfo = ref ProtocolHelper.CreatePacket<GameServerInfo>(out byte[] data);
      serverInfo.Host = "192.168.1.100";
      serverInfo.Port = 2004;
      CollectionAssert.AreEqual(GameServerInfoPacket, data);

      serverInfo = ProtocolHelper.ParsePacket<GameServerInfo>(GameServerInfoPacket);
      Assert.AreEqual("192.168.1.100", serverInfo.Host);
      Assert.AreEqual(2004, serverInfo.Port);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void GameServerInfo_Serialize_Fails_When_MaxLength_Is_Exceeded() {
      ref var serverInfo = ref ProtocolHelper.CreatePacket<GameServerInfo>(out byte[] data);
      serverInfo.Host = "192.168.255.255-";
    }

    [TestMethod]
    public void GameServerUnavailable_Serialization() {
      ref var unavailable = ref ProtocolHelper.CreatePacket<GameServerUnavailable>(out byte[] data);
      unavailable.ServerCode = 1337;
      CollectionAssert.AreEqual(GameServerUnavailablePacket, data);

      unavailable = ProtocolHelper.ParsePacket<GameServerUnavailable>(GameServerUnavailablePacket);
      Assert.AreEqual(1337, unavailable.ServerCode);
    }

    [TestMethod]
    public void GameServerList_Serialize() {
      ref var list = ref ProtocolHelper.CreatePacket<GameServerList, GameServerList.GameServer>(
        GameServerListEntries.Length,
        out byte[] data,
        out Span<GameServerList.GameServer> servers);
      Assert.AreEqual(GameServerListEntries.Length, list.Count);
      Assert.AreEqual(GameServerListEntries.Length, servers.Length);

      foreach (var (index, (code, load, preparing)) in GameServerListEntries.Select((e, i) => (i, e))) {
        servers[index].Code = code;
        servers[index].Load = load;
        servers[index].IsPreparing = preparing;
      }

      CollectionAssert.AreEqual(GameServerListPacket, data);
    }

    [TestMethod]
    public void GameServerList_Deserialize() {
      ref var list = ref ProtocolHelper.ParsePacket<GameServerList, GameServerList.GameServer>(
        GameServerListPacket,
        out Span<GameServerList.GameServer> servers);

      Assert.AreEqual(GameServerListEntries.Length, list.Count);
      Assert.AreEqual(GameServerListEntries.Length, servers.Length);

      foreach (var (index, (code, load, preparing)) in GameServerListEntries.Select((e, i) => (i, e))) {
        Assert.AreEqual(preparing, servers[index].IsPreparing);
        Assert.AreEqual(code, (ushort)servers[index].Code);
        Assert.AreEqual(load, servers[index].Load);
      }
    }
  }
}

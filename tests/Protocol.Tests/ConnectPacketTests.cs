using System;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Muwesome.Packet;
using Muwesome.Packet.Utility;
using Muwesome.Protocol.Connect.Client;
using Muwesome.Protocol.Connect.Server;

namespace Muwesome.Protocol.Tests {
  [TestClass]
  public class ConnectPacketTests {
    private static readonly byte[] ConnectResultPacketSuccess = new byte[] { 0xC1, 0x04, 0x00, 0x01 };
    private static readonly byte[] GameServerUnavailablePacket = new byte[] { 0xC1, 0x06, 0xF4, 0x05, 0x39, 0x05 };
    private static readonly byte[] GameServerInfoPacket = Convert.FromBase64String("wRb0AzE5Mi4xNjguMS4xMDAAAADUBw==");
    private static readonly byte[] GameServerListPacket = Convert.FromBase64String("wgAX9AYABAEAIAACAAAAAwAgAAQAgAA=");
    private static readonly (ushort, float, bool)[] GameServerListEntries = new(ushort, float, bool)[] { (1, 0.32f, false), (2, 0f, false), (3, 0.32f, false), (4, 0f, true) };

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Packet_Identifier_Validation_Fails_When_Mismatch() {
      PacketHelper.ParsePacket<GameServerUnavailable>(GameServerInfoPacket);
    }

    [TestMethod]
    public void ConnectResult_Serialization() {
      ref var result = ref PacketHelper.CreatePacket<ConnectResult>(out byte[] data);
      result.Success = true;
      CollectionAssert.AreEqual(ConnectResultPacketSuccess, data);

      result = PacketHelper.ParsePacket<ConnectResult>(ConnectResultPacketSuccess);
      Assert.IsTrue(result.Success);
    }

    [TestMethod]
    public void GameServerInfo_Serialization() {
      ref var serverInfo = ref PacketHelper.CreatePacket<GameServerInfo>(out byte[] data);
      serverInfo.Host = "192.168.1.100";
      serverInfo.Port = 2004;
      CollectionAssert.AreEqual(GameServerInfoPacket, data);

      serverInfo = PacketHelper.ParsePacket<GameServerInfo>(GameServerInfoPacket);
      Assert.AreEqual("192.168.1.100", serverInfo.Host);
      Assert.AreEqual(2004, serverInfo.Port);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void GameServerInfo_Serialize_Fails_When_MaxLength_Is_Exceeded() {
      ref var serverInfo = ref PacketHelper.CreatePacket<GameServerInfo>(out byte[] data);
      serverInfo.Host = "192.168.255.255-";
    }

    [TestMethod]
    public void GameServerUnavailable_Serialization() {
      ref var unavailable = ref PacketHelper.CreatePacket<GameServerUnavailable>(out byte[] data);
      unavailable.ServerCode = 1337;
      CollectionAssert.AreEqual(GameServerUnavailablePacket, data);

      unavailable = PacketHelper.ParsePacket<GameServerUnavailable>(GameServerUnavailablePacket);
      Assert.AreEqual(1337, unavailable.ServerCode);
    }

    [TestMethod]
    public void GameServerList_Serialize() {
      ref var list = ref PacketHelper.CreatePacket<GameServerList, GameServerList.GameServer>(
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
      ref var list = ref PacketHelper.ParsePacket<GameServerList, GameServerList.GameServer>(
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
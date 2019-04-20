using System;
using System.Collections.Generic;
using System.Linq;
using Muwesome.Packet;
using OneOf;

namespace Muwesome.Protocol {
  public abstract class ConfigurablePacketHandler<T> : IPacketHandler<T> {
    private readonly PacketToHandlerDictionary _packetHandlers = new PacketToHandlerDictionary();

    /// <inheritdoc />
    public virtual bool HandlePacket(T sender, Span<byte> packetData) {
      var packet = new PacketView(packetData);
      var handler = GetHandlerForPacket(packet);

      if (handler == null) {
        return false;
      }

      return handler.HandlePacket(sender, packet.Data);
    }

    protected void Register<TPacket>(IPacketHandler<T> handler) where TPacket: PacketDefinition, new() {
      InsertHandlerForPacket<TPacket>(handler);
    }

    private IPacketHandler<T> GetHandlerForPacket(PacketView packet) {
      var handlers = _packetHandlers;
      foreach (byte key in packet.Identifier) {
        if (!handlers.TryGetValue(key, out OneOf<PacketToHandlerDictionary, IPacketHandler<T>> value)) {
          break;
        }

        if (value.IsT1) {
          return value.AsT1;
        }

        handlers = value.AsT0;
      }

      return null;
    }

    private void InsertHandlerForPacket<TPacket>(IPacketHandler<T> handler) where TPacket: PacketDefinition, new() {
      // TODO: DONT REQUIRE NEW() HERE FFFSFSSFFSSFFSSFFS
      // TODO: AND IMPROVE EXCEPTION (SHOW CONFLICTING HANDLER!!!)
      var packet = new TPacket();
      var identifier = packet.Identifier.ToArray();

      var handlers = _packetHandlers;
      for (int i = 0; i < identifier.Length; i++) {
        bool isLastIdentifier = i == identifier.Length - 1;
        bool identifierExists = handlers.ContainsKey(identifier[i]);

        if (isLastIdentifier) {
          if (identifierExists) {
            throw new ConflictingPacketHandlersException(packet);
          }

          var value = OneOf<PacketToHandlerDictionary, IPacketHandler<T>>.FromT1(handler);
          handlers.Add(identifier[i], value);
        } else if (identifierExists) {
          handlers = handlers[identifier[i]].Match(
            dictionary => dictionary,
            _ => throw new ConflictingPacketHandlersException(packet)
          );
        } else {
          var nestedHandlers = new PacketToHandlerDictionary();
          handlers.Add(identifier[i], nestedHandlers);
          handlers = nestedHandlers;
        }
      }
    }

    private class PacketToHandlerDictionary : Dictionary<byte, OneOf<PacketToHandlerDictionary, IPacketHandler<T>>> { }
  }
}
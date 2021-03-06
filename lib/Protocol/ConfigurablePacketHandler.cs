using System;
using System.Collections.Generic;
using System.Linq;
using Muwesome.Packet;
using Muwesome.Packet.Utility;
using Muwesome.Protocol.Utility;
using OneOf;

namespace Muwesome.Protocol {
  public abstract class ConfigurablePacketHandler<T> : IPacketHandler<T> {
    private readonly PacketToHandlerDictionary packetHandlers = new PacketToHandlerDictionary();

    /// <inheritdoc />
    public virtual bool HandlePacket(T sender, Span<byte> packetData) {
      var packet = new PacketView(packetData);
      var handler = this.GetHandlerForPacket(packet);

      if (handler == null) {
        return false;
      }

      return handler.HandlePacket(sender, packetData);
    }

    /// <summary>Registers a new handler for a packet type.</summary>
    protected void RegisterHandler<TPacket>(IPacketHandler<T> handler)
        where TPacket : IPacket {
      this.InsertHandlerForPacket(PacketIdentifier.Get<TPacket>(), handler);
    }

    /// <summary>Registers a new handler for a packet.</summary>
    protected void RegisterHandler(PacketIdentifier packet, IPacketHandler<T> handler) {
      this.InsertHandlerForPacket(packet, handler);
    }

    private IPacketHandler<T> GetHandlerForPacket(PacketView packet) {
      var handlers = this.packetHandlers;
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

    private void InsertHandlerForPacket(PacketIdentifier packet, IPacketHandler<T> handler) {
      // TODO: Throw improved exception (show conflicting packets)
      var identifier = packet.Identifier;

      var handlers = this.packetHandlers;
      for (int i = 0; i < identifier.Count; i++) {
        bool isLastIdentifier = i == identifier.Count - 1;
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
            _ => throw new ConflictingPacketHandlersException(packet));
        } else {
          var nestedHandlers = new PacketToHandlerDictionary();
          handlers.Add(identifier[i], nestedHandlers);
          handlers = nestedHandlers;
        }
      }
    }

    // TODO: Use sorted set instead?
    private class PacketToHandlerDictionary : Dictionary<byte, OneOf<PacketToHandlerDictionary, IPacketHandler<T>>> {
    }
  }
}
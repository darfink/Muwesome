using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Muwesome.GameLogic.Actions;
using Muwesome.GameServer.Protocol.Dispatchers;
using Muwesome.MethodDelegate.Extensions;
using Muwesome.Packet;

namespace Muwesome.GameServer.Protocol {
  /// <summary>A client packet dispatcher.</summary>
  internal class ClientPacketDispatcher {
    private readonly IList<PacketDispatcher> dispatchers = new List<PacketDispatcher>();

    /// <summary>Initializes a new instance of the <see cref="ClientPacketDispatcher"/> class.</summary>
    public ClientPacketDispatcher(IEnumerable<PacketDispatcher> dispatchers) =>
      this.dispatchers = dispatchers.ToList();

    /// <summary>Registers all dispatchers bound to a client.</summary>
    public IEnumerable<Delegate> CreateDispatches(Client client) {
      // TODO: Log each dispatch
      return this.dispatchers.SelectMany(dispatcher => dispatcher.GetMethodDelegates(ParameterResolver));

      object ParameterResolver(ParameterInfo parameter) {
        if (parameter.ParameterType != typeof(Client)) {
          throw new Exception($"Unresolved dispatch parameter {parameter}");
        }

        return client;
      }
    }
  }
}
using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Muwesome.Packet {
  /// <summary>Defines a packet type.</summary>
  public interface IPacket { }

  /// <summary>Defines a fixed sized packet type.</summary>
  public interface IFixedSizedPacket : IPacket { }

  /// <summary>Defines a variable sized packet type.</summary>
  public interface IVariableSizedPacket : IPacket { }
}
using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Muwesome.Packet {
  /// <summary>Defines a packet.</summary>
  public interface IPacket { }

  /// <summary>Defines a fixed sized packet.</summary>
  public interface IFixedSizedPacket : IPacket { }

  /// <summary>Defines a variable sized packet.</summary>
  public interface IVariableSizedPacket : IPacket { }
}
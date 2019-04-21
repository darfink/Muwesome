using System;
using System.Collections.Generic;

namespace Muwesome.Packet {
  [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
  public sealed class PacketAttribute : Attribute {
    /// <summary>Constructs a new packet attribute.</summary>
    public PacketAttribute(byte type, byte code, params byte[] subcode) {
      Type = type;
      Code = code;
      Subcode = subcode;
    }

    /// <summary>Gets or sets the attribute's size.</summary>
    public int Size { get; set; }

    /// <summary>Gets the attribute's type.</summary>
    internal byte Type { get; }

    /// <summary>Gets the attribute's code.</summary>
    internal byte Code { get; }

    /// <summary>Gets the attribute's subcode.</summary>
    internal byte[] Subcode { get; }

    /// <summary>Gets the packet's identifier.</summary>
    internal IEnumerable<byte> Identifier {
      get {
        yield return Type;
        yield return Code;
        foreach (var subcode in Subcode) {
          yield return subcode;
        }
      }
    }
  }

  public static class PacketFor<T> {
    private static readonly PacketAttribute _definition;

    /// <summary>Caches the packet's attributes.</summary>
    static PacketFor() =>
      _definition = (PacketAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(PacketAttribute), false);

    /// <summary>Gets the packet's name.</summary>
    public static string Name => typeof(T).Name;

    /// <summary>Gets the packet's size.</summary>
    public static int? Size => _definition.Size == 0 ? null : (int?)_definition.Size;

    /// <summary>Gets the packet's type.</summary>
    public static byte Type => _definition.Type;

    /// <summary>Gets the packet's code.</summary>
    public static byte Code => _definition.Code;

    /// <summary>Gets the packet's subcode.</summary>
    public static byte[] Subcode => _definition.Subcode;

    /// <summary>Gets the packet's identifier.</summary>
    public static IEnumerable<byte> Identifier => _definition.Identifier;
  }
}
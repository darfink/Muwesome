using System;
using Muwesome.DomainModel.Configuration;

namespace Muwesome.DomainModel.Entities {
  /// <summary>The character of an account.</summary>
  public class Character : Identifiable {
    /// <summary>Gets or sets the slot.</summary>
    public byte Slot { get; set; }

    /// <summary>Gets or sets the name.</summary>
    public string Name { get; set; }

    /// <summary>Gets or sets the class.</summary>
    public CharacterClass Class { get; set; }

    /// <summary>Gets or sets the current map.</summary>
    public MapDefinition CurrentMap { get; set; }

    /// <summary>Gets or sets the position.</summary>
    public Point Position { get; set; }

    /// <summary>Gets or sets the experience.</summary>
    public long Experience { get; set; }

    /// <summary>Gets or sets the kill count.</summary>
    public int KillCount { get; set; }

    /// <summary>Gets or sets the registration date.</summary>
    public DateTime CreationDate { get; set; }
  }
}
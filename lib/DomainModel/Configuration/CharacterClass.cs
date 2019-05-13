using System;

namespace Muwesome.DomainModel.Configuration {
  /// <summary>A character class configuration.</summary>
  public class CharacterClass {
    /// <summary>Gets or sets the name.</summary>
    public string Name { get; set; }

    /// <summary>Gets or sets the number of points received per level.</summary>
    public int PointsPerLevel { get; set; }

    /// <summary>Gets or sets the start map for new characters of this class.</summary>
    public MapDefinition HomeMap { get; set; }
  }
}
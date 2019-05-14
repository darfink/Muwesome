using System;

namespace Muwesome.DomainModel.Configuration {
  /// <summary>A character class configuration.</summary>
  public class CharacterClass : Entity {
    public CharacterClass(string name) {
      this.Name = name;
    }

    protected CharacterClass() {
    }

    /// <summary>Gets or sets the name.</summary>
    public virtual string Name { get; set; }

    /// <summary>Gets or sets the number of points received per level.</summary>
    public virtual int PointsPerLevel { get; set; }

    /// <summary>Gets or sets the start map for new characters of this class.</summary>
    public virtual MapDefinition HomeMap { get; set; }
  }
}
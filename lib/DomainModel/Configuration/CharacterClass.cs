using System;

namespace Muwesome.DomainModel.Configuration {
  /// <summary>A character class configuration.</summary>
  public class CharacterClass : Identifiable {
    /// <summary>Initializes a new instance of the <see cref="CharacterClass"/> class.</summary>
    public CharacterClass(string name, byte number, MapDefinition homeMap) {
      this.HomeMap = homeMap;
      this.Number = number;
      this.Name = name;
    }

    /// <summary>Initializes a new instance of the <see cref="CharacterClass"/> class.</summary>
    protected CharacterClass() {
    }

    /// <summary>Gets or sets the number.</summary>
    public virtual byte Number { get; set; }

    /// <summary>Gets or sets the name.</summary>
    public virtual string Name { get; set; }

    /// <summary>Gets or sets the number of points received per level.</summary>
    public virtual int PointsPerLevel { get; set; }

    /// <summary>Gets or sets the start map for new characters of this class.</summary>
    public virtual MapDefinition HomeMap { get; set; }
  }
}
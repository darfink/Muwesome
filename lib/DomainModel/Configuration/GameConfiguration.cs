using System.Collections.Generic;

namespace Muwesome.DomainModel.Configuration {
  /// <summary>A game configuration.</summary>
  public class GameConfiguration : Identifiable {
    /// <summary>Initializes a new instance of the <see cref="GameConfiguration"/> class.</summary>
    public GameConfiguration() {
      this.CharacterClasses = new List<CharacterClass>();
      this.ItemDefinitions = new List<ItemDefinition>();
      this.MapDefinitions = new List<MapDefinition>();
    }

    /// <summary>Gets or sets the character classes.</summary>
    public virtual IList<CharacterClass> CharacterClasses { get; set; }

    /// <summary>Gets or sets the item definitions.</summary>
    public virtual IList<ItemDefinition> ItemDefinitions { get; set; }

    /// <summary>Gets or sets the map definitions.</summary>
    public virtual IList<MapDefinition> MapDefinitions { get; set; }
  }
}
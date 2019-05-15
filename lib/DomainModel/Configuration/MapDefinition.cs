using System;
using System.Collections.Generic;
using Muwesome.DomainModel.Components;

namespace Muwesome.DomainModel.Configuration {
  /// <summary>A map definition.</summary>
  public class MapDefinition : Identifiable {
    /// <summary>Initializes a new instance of the <see cref="MapDefinition"/> class.</summary>
    public MapDefinition(string name, short number, MapTerrain terrain) {
      this.Number = number;
      this.Name = name;
      this.Terrain = terrain;
      this.Gates = new List<MapGate>();
    }

    /// <summary>Initializes a new instance of the <see cref="MapDefinition"/> class.</summary>
    protected MapDefinition() {
    }

    /// <summary>Gets or sets the number.</summary>
    public virtual short Number { get; set; }

    /// <summary>Gets or sets the name.</summary>
    public virtual string Name { get; set; }

    /// <summary>Gets or sets the terrain.</summary>
    public virtual MapTerrain Terrain { get; set; }

    /// <summary>Gets or sets the gates.</summary>
    public virtual IList<MapGate> Gates { get; set; }
  }
}
using System;
using System.Collections.Generic;
using Muwesome.DomainModel.Components;

namespace Muwesome.DomainModel.Configuration {
  /// <summary>A map definition.</summary>
  public class MapDefinition : Entity {
    /// <summary>Gets or sets the number.</summary>
    public virtual short Number { get; set; }

    /// <summary>Gets or sets the terrain.</summary>
    public virtual MapTerrain Terrain { get; set; }

    /// <summary>Gets or sets the gates.</summary>
    public virtual IList<MapGate> Gates { get; set; }
  }
}
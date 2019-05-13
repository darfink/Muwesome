using System;
using System.Collections.Generic;

namespace Muwesome.DomainModel.Configuration {
  /// <summary>A map definition.</summary>
  public class MapDefinition {
    /// <summary>Gets or sets the number.</summary>
    public short Number { get; set; }

    /// <summary>Gets or sets the terrain.</summary>
    public MapTerrain Terrain { get; set; }

    /// <summary>Gets or sets the gates.</summary>
    public IList<MapGate> Gates { get; set; }
  }
}
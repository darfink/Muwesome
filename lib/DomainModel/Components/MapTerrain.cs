using System.Collections.Generic;

namespace Muwesome.DomainModel.Components {
  public class MapTerrain {
    /// <summary>Gets or sets the width.</summary>
    public int Width { get; set; }

    /// <summary>Gets or sets the height.</summary>
    public int Height { get; set; }

    public byte[,] Terrain { get; set; }
  }
}
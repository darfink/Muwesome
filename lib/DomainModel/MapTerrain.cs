using System.Collections.Generic;

namespace Muwesome.DomainModel {

  public class MapTerrain {
    public int Width { get; set; }

    public int Height { get; set; }

    private byte[] Data { get; set; }

    public byte this[int x, int y] => this.Data[x + (y * this.Width)];
  }
}
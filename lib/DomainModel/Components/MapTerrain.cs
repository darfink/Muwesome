using System;
using System.Collections.Generic;
using System.IO;

namespace Muwesome.DomainModel.Components {
  public class MapTerrain {
    public MapTerrain(int width, int height) {
      this.Width = width;
      this.Height = height;
      this.Terrain = new byte[width * height];
    }

    protected MapTerrain() {
    }

    /// <summary>Gets or sets the width.</summary>
    public int Width { get; protected set; }

    /// <summary>Gets or sets the height.</summary>
    public int Height { get; protected set; }

    /// <summary>Gets or sets the terrain.</summary>
    public byte[] Terrain { get; protected set; }

    public static MapTerrain FromStream(Stream stream) {
      stream.ReadByte();
      int width = stream.ReadByte() + 1;
      int height = stream.ReadByte() + 1;
      var map = new MapTerrain(width, height);

      int bytesRead = stream.Read(map.Terrain, 0, map.Terrain.Length);

      if (bytesRead != map.Terrain.Length || stream.ReadByte() != -1) {
        throw new InvalidDataException(nameof(stream));
      }

      return map;
    }
  }
}
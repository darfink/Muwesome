using System.Collections.Generic;

namespace Muwesome.DomainModel.Entities {
  /// <summary>A storage of items.</summary>
  public class ItemStorage : Identifiable {
    /// <summary>Initializes a new instance of the <see cref="ItemStorage"/> class.</summary>
    public ItemStorage(byte width, byte height) {
      this.Width = width;
      this.Height = height;
      this.Items = new Dictionary<byte, Item>();
    }

    /// <summary>Initializes a new instance of the <see cref="ItemStorage"/> class.</summary>
    protected ItemStorage() {
    }

    /// <summary>Gets or sets the width.</summary>
    public byte Width { get; set; }

    /// <summary>Gets or sets the height.</summary>
    public byte Height { get; set; }

    /// <summary>Gets or sets the amount of money.</summary>
    public int Money { get; set; }

    /// <summary>Gets or sets the items.</summary>
    public IDictionary<byte, Item> Items { get; set; }
  }
}
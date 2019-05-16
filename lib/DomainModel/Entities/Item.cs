using Muwesome.DomainModel.Configuration;

namespace Muwesome.DomainModel.Entities {
  /// <summary>A representation of an item.</summary>
  public class Item : Identifiable {
    /// <summary>Initializes a new instance of the <see cref="Item"/> class.</summary>
    public Item(ItemDefinition definition) {
      this.Definition = definition;
    }

    /// <summary>Initializes a new instance of the <see cref="Item"/> class.</summary>
    protected Item() {
    }

    /// <summary>Gets or sets the definition.</summary>
    public ItemDefinition Definition { get; set; }

    /// <summary>Gets or sets the durability.</summary>
    public byte Durability { get; set; }
  }
}
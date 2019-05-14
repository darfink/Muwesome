using System.Collections.Generic;

namespace Muwesome.DomainModel.Entities {
  /// <summary>A storage of items.</summary>
  public class ItemStorage : Entity {
    /// <summary>Gets or sets the width.</summary>
    public virtual byte Width { get; set; }

    /// <summary>Gets or sets the height.</summary>
    public virtual byte Height { get; set; }

    /// <summary>Gets or sets the amount of money.</summary>
    public virtual int Money { get; set; }

    /// <summary>Gets or sets the items.</summary>
    public virtual IDictionary<byte, Item> Items { get; set; }
  }
}
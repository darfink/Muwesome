using Muwesome.DomainModel.Configuration;

namespace Muwesome.DomainModel.Entities {
  public class Item : Entity {
    /// <summary>Gets or sets the definition.</summary>
    public virtual ItemDefinition Definition { get; set; }

    /// <summary>Gets or sets the durability.</summary>
    public virtual byte Durability { get; set; }
  }
}
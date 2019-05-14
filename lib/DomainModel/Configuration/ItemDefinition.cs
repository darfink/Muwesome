using Muwesome.DomainModel.Components;

namespace Muwesome.DomainModel.Configuration {
  /// <summary>A item definition.</summary>
  public class ItemDefinition : Entity {
    /// <summary>Gets or sets the code.</summary>
    public virtual ItemCode Code { get; set; }

    public virtual string Name { get; set; }

    public virtual byte Width { get; set; }

    public virtual byte Height { get; set; }

    public virtual byte BaseValue { get; set; }

    public virtual byte BaseDurability { get; set; }
  }
}
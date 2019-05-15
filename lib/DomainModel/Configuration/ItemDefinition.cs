using Muwesome.DomainModel.Components;

namespace Muwesome.DomainModel.Configuration {
  /// <summary>A item definition.</summary>
  public class ItemDefinition : Identifiable {
    /// <summary>Initializes a new instance of the <see cref="ItemDefinition"/> class.</summary>
    public ItemDefinition(string name, ItemCode code) {
      this.Name = name;
      this.Code = code;
    }

    /// <summary>Initializes a new instance of the <see cref="ItemDefinition"/> class.</summary>
    protected ItemDefinition() {
    }

    /// <summary>Gets or sets the code.</summary>
    public virtual ItemCode Code { get; set; }

    /// <summary>Gets or sets the name.</summary>
    public virtual string Name { get; set; }

    /// <summary>Gets or sets the width.</summary>
    public virtual byte Width { get; set; }

    /// <summary>Gets or sets the height.</summary>
    public virtual byte Height { get; set; }

    /// <summary>Gets or sets the base value.</summary>
    public virtual byte BaseValue { get; set; }

    /// <summary>Gets or sets the base durability.</summary>
    public virtual byte BaseDurability { get; set; }
  }
}
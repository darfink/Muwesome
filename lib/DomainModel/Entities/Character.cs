using System;
using System.Collections.Generic;
using Muwesome.DomainModel.Components;
using Muwesome.DomainModel.Configuration;

namespace Muwesome.DomainModel.Entities {
  /// <summary>A character of an account.</summary>
  public class Character : Entity {
    public Character(string name, byte slot) {
      this.Slot = slot;
      this.Name = name;
      this.Equipment = new Dictionary<EquipmentSlot, Item>();
      this.CreationDate = DateTime.UtcNow;
    }

    protected Character() {
    }

    /// <summary>Gets or sets the slot.</summary>
    public virtual byte Slot { get; set; }

    /// <summary>Gets or sets the name.</summary>
    public virtual string Name { get; set; }

    /// <summary>Gets or sets the class.</summary>
    public virtual CharacterClass Class { get; set; }

    /// <summary>Gets or sets the current map.</summary>
    public virtual MapDefinition CurrentMap { get; set; }

    /// <summary>Gets or sets the position.</summary>
    public virtual Point Position { get; set; }

    /// <summary>Gets or sets the equipment.</summary>
    public virtual IDictionary<EquipmentSlot, Item> Equipment { get; set; }

    /// <summary>Gets or sets the inventory.</summary>
    public virtual ItemStorage Inventory { get; set; }

    /// <summary>Gets or sets the personal shop.</summary>
    public virtual ItemStorage PersonalShop { get; set; }

    /// <summary>Gets or sets the experience.</summary>
    public virtual long Experience { get; set; }

    /// <summary>Gets or sets the kill count.</summary>
    public virtual int KillCount { get; set; }

    /// <summary>Gets or sets the registration date.</summary>
    public virtual DateTime CreationDate { get; set; }
  }
}
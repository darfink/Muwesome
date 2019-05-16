using System;
using System.Collections.Generic;
using Muwesome.DomainModel.Components;
using Muwesome.DomainModel.Configuration;

namespace Muwesome.DomainModel.Entities {
  /// <summary>A character of an account.</summary>
  public class Character : Identifiable {
    /// <summary>Initializes a new instance of the <see cref="Character"/> class.</summary>
    public Character(string name, CharacterClass @class, ItemStorage inventory) {
      this.Name = name;
      this.Class = @class;
      this.CurrentMap = @class.HomeMap;
      this.Equipment = new Dictionary<EquipmentSlot, Item>();
      this.Inventory = inventory;
      this.CreationDate = DateTime.UtcNow;
    }

    /// <summary>Initializes a new instance of the <see cref="Character"/> class.</summary>
    protected Character() {
    }

    /// <summary>Gets or sets the slot.</summary>
    public byte Slot { get; set; }

    /// <summary>Gets or sets the name.</summary>
    public string Name { get; set; }

    /// <summary>Gets or sets the class.</summary>
    public CharacterClass Class { get; set; }

    /// <summary>Gets or sets the current map.</summary>
    public MapDefinition CurrentMap { get; set; }

    /// <summary>Gets or sets the position.</summary>
    public Point Position { get; set; }

    /// <summary>Gets or sets the equipment.</summary>
    public IDictionary<EquipmentSlot, Item> Equipment { get; set; }

    /// <summary>Gets or sets the inventory.</summary>
    public ItemStorage Inventory { get; set; }

    /// <summary>Gets or sets the personal shop.</summary>
    public ItemStorage PersonalShop { get; set; }

    /// <summary>Gets or sets the experience.</summary>
    public long Experience { get; set; }

    /// <summary>Gets or sets the kill count.</summary>
    public int KillCount { get; set; }

    /// <summary>Gets or sets the registration date.</summary>
    public DateTime CreationDate { get; set; }
  }
}
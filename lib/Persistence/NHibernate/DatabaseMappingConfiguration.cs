using System;
using System.Collections.Generic;
using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Muwesome.DomainModel;
using Muwesome.DomainModel.Components;
using Muwesome.DomainModel.Configuration;
using Muwesome.DomainModel.Entities;
using Muwesome.Persistence.NHibernate.Utility;

namespace Muwesome.Persistence.NHibernate {
  public class DatabaseMappingConfiguration : DefaultAutomappingConfiguration {
    /// <inheritdoc />
    public override bool ShouldMap(Type type) => type.IsSubclassOf(typeof(Identifiable));

    /// <inheritdoc />
    public override bool IsComponent(Type type) => type.Namespace.EndsWith("Components");

    public class AccountMappingOverride : IAutoMappingOverride<Account> {
      public void Override(AutoMapping<Account> mapping) {
        mapping.Map(m => m.Username).Unique().Length(10);
        mapping.Map(m => m.SecurityCode).Length(10);
        mapping.Map(m => m.Mail).Nullable();
        mapping.References(m => m.Vault).Nullable();
        mapping.Map(m => m.RegistrationDate).Default("CURRENT_TIMESTAMP");
        mapping.HasManyNonNullable(m => m.Characters, m => m.UniqueKey($"{nameof(Account)}{nameof(Character.Slot)}"));
      }
    }

    public class CharacterMappingOverride : IAutoMappingOverride<Character> {
      public void Override(AutoMapping<Character> mapping) {
        mapping.Map(m => m.Slot).UniqueKey($"{nameof(Account)}{nameof(Character.Slot)}");
        mapping.Map(m => m.Name).Unique().Length(10);
        mapping.Map(m => m.CreationDate).Default("CURRENT_TIMESTAMP");
        mapping.References(m => m.PersonalShop).Nullable();
        mapping.Component(m => m.Position).ColumnPrefix(nameof(Character.Position));
        mapping.HasManyToMany(m => m.Equipment)
          .AsMap(x => x.Key)
          .AsSimpleAssociation(nameof(EquipmentSlot), $"{nameof(Item)}Id")
          .Table($"Equipped{nameof(Item)}");
      }
    }

    public class ItemStorageMappingOverride : IAutoMappingOverride<ItemStorage> {
      public void Override(AutoMapping<ItemStorage> mapping) {
        mapping.HasManyToMany(m => m.Items)
          .AsMap(x => x.Key)
          .AsSimpleAssociation("Slot", $"{nameof(Item)}Id")
          .Table($"Stored{nameof(Item)}");
      }
    }

    public class MapDefinitionMappingOverride : IAutoMappingOverride<MapDefinition> {
      public void Override(AutoMapping<MapDefinition> mapping) {
        mapping.Component(m => m.Terrain).ColumnPrefix(null);
        mapping.HasManyNonNullable(m => m.Gates);
      }
    }

    public class MapGateMappingOverride : IAutoMappingOverride<MapGate> {
      public void Override(AutoMapping<MapGate> mapping) {
        mapping.Component(m => m.Area).ColumnPrefix(null);
      }
    }

    public class ItemCodeMappingOverride : IAutoMappingOverride<ItemCode> {
      public void Override(AutoMapping<ItemCode> mapping) {
        mapping.IgnoreProperty(m => m.Group);
        mapping.IgnoreProperty(m => m.Number);
      }
    }

    public class RectangleMappingOverride : IAutoMappingOverride<Rectangle> {
      public void Override(AutoMapping<Rectangle> mapping) {
        mapping.IgnoreProperty(m => m.Right);
        mapping.IgnoreProperty(m => m.Bottom);
        mapping.Map(m => m.Left).Column("X");
        mapping.Map(m => m.Top).Column("Y");
      }
    }
  }
}
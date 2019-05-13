using System;
using Microsoft.EntityFrameworkCore;
using Muwesome.DomainModel;
using Muwesome.DomainModel.Configuration;
using Muwesome.DomainModel.Entities;

namespace Muwesome.Persistence.EntityFramework {
  internal class DatabaseContext : DbContext {
    public DatabaseContext(DbContextOptions options)
        : base(options) {
    }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<CharacterClass>(@class => {
        @class.Property<Guid>("Id");
        @class.HasKey("Id");
        @class.Property(c => c.Name).IsRequired();
        @class.HasIndex(c => c.Name).IsUnique();
      });

      modelBuilder.Entity<MapDefinition>(map => {
        map.Property<Guid>("Id");
        map.HasKey("Id");
        map.OwnsOne(typeof(MapTerrain), nameof(MapDefinition.Terrain), builder => {
          builder.Property(nameof(MapTerrain.Width)).HasColumnName(nameof(MapTerrain.Width));
          builder.Property(nameof(MapTerrain.Height)).HasColumnName(nameof(MapTerrain.Height));
          builder.Property("Data").HasColumnName("Terrain").IsRequired();
        });
      });

      modelBuilder.Entity<MapGate>(mapGate => {
        mapGate.Property<Guid>("Id");
        mapGate.HasKey("Id");
      });

      modelBuilder.Entity<Account>(account => {
        account.HasKey(a => a.Id);
        account.Property(a => a.Username).HasMaxLength(10).IsRequired();
        account.HasIndex(a => a.Username).IsUnique();
        account.Property(a => a.SecurityCode).HasMaxLength(10).IsRequired();
        account.Property(a => a.RegistrationDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
        account.OwnsMany(a => a.Characters);
      });

      modelBuilder.Entity<Character>(character => {
        character.HasKey(c => c.Id);
        character.HasIndex($"{nameof(Account)}Id", nameof(Character.Slot)).IsUnique();
        character.Property(a => a.Name).HasMaxLength(10).IsRequired();
        character.HasIndex(a => a.Name).IsUnique();
        character.Property(e => e.CreationDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
        character.OwnsOne(typeof(Point), nameof(Character.Position), builder => {
          builder.Property(nameof(Point.X)).HasColumnName("PositionX");
          builder.Property(nameof(Point.Y)).HasColumnName("PositionY");
        });

        var mapId = character.Property<Guid>("CurrentMapId");
        character.HasOne(c => c.CurrentMap).WithMany().HasForeignKey(mapId.Metadata.Name);

        var classId = character.Property<Guid>("ClassId");
        character.HasOne(c => c.Class).WithMany().HasForeignKey(classId.Metadata.Name);
      });
    }
  }
}
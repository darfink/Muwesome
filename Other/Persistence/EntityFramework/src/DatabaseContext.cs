using Microsoft.EntityFrameworkCore;
using Muwesome.DomainModel.Entities;

namespace Muwesome.Persistence.EntityFramework {
  internal class DatabaseContext : DbContext {
    public DatabaseContext(DbContextOptions options)
        : base(options) {
    }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
      base.OnModelCreating(modelBuilder);
      modelBuilder.Entity<Account>(account => {
        account.Property(e => e.Username).HasMaxLength(10).IsRequired();
        account.HasIndex(e => e.Username).IsUnique();
        account.Property(e => e.SecurityCode).HasMaxLength(10).IsRequired();
        account.Property(e => e.RegistrationDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
      });
    }
  }
}
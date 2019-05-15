using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FluentNHibernate.Automapping;
using FluentNHibernate.Mapping;

namespace Muwesome.Persistence.NHibernate.Utility {
  public static class AutoMappingExtensions {
    /// <summary>Creates a one-to-many relationship with non-nullable foreign keys.</summary>
    public static OneToManyPart<TChild> HasManyNonNullable<T, TChild>(
        this AutoMapping<T> mapping,
        Expression<Func<T, IEnumerable<TChild>>> memberExpression,
        Action<ColumnPart> customKeyMapping = null) {
      customKeyMapping = customKeyMapping ?? (m => { });

      return mapping
        .HasMany(memberExpression)
        .KeyColumns
          .Add($"{typeof(T).Name}Id", m => customKeyMapping(m.Not.Nullable()))
        .Not.Inverse()
        .Not.KeyNullable()
        .Not.KeyUpdate()
        .Cascade.AllDeleteOrphan();
    }
  }
}
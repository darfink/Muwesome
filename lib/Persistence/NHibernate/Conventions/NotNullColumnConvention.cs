using System;
using FluentNHibernate.Conventions;

namespace Muwesome.Persistence.NHibernate.Conventions {
  public class NotNullColumnConvention : IPropertyConvention {
    public void Apply(FluentNHibernate.Conventions.Instances.IPropertyInstance instance) {
      bool isNullable = Nullable.GetUnderlyingType(instance.Property.PropertyType) != null;

      if (!isNullable || instance.Property.PropertyType == typeof(string)) {
        instance.Not.Nullable();
      }
    }
  }
}
using System;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Muwesome.Persistence.NHibernate.Conventions {
  public class NotNullConvention :
      IPropertyConvention,
      IReferenceConvention,
      IHasManyConvention,
      IClassConvention {
    /// <inheritdoc />
    public void Apply(IPropertyInstance instance) {
      bool isNullable = Nullable.GetUnderlyingType(instance.Property.PropertyType) != null;

      if (!isNullable || instance.Property.PropertyType == typeof(string)) {
        instance.Not.Nullable();
      }
    }

    /// <inheritdoc />
    public void Apply(IManyToOneInstance instance) =>
      instance.Not.Nullable();

    /// <inheritdoc />
    public void Apply(IOneToManyCollectionInstance instance) {
      instance.Not.LazyLoad();
    }

    /// <inheritdoc />
    public void Apply(IClassInstance instance) {
      instance.Not.LazyLoad();
    }
  }
}
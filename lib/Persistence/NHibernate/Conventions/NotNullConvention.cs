using System;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace Muwesome.Persistence.NHibernate.Conventions {
  internal class NotNullConvention :
      IPropertyConventionAcceptance,
      IPropertyConvention,
      IReferenceConvention {
    /// <inheritdoc />
    public void Accept(IAcceptanceCriteria<IPropertyInspector> critera) {
      critera.Expect(property => {
        bool isNullable = Nullable.GetUnderlyingType(property.Property.PropertyType) != null;
        return !isNullable || property.Property.PropertyType == typeof(string);
      });
    }

    /// <inheritdoc />
    public void Apply(IPropertyInstance instance) => instance.Not.Nullable();

    /// <inheritdoc />
    public void Apply(IManyToOneInstance instance) => instance.Not.Nullable();
  }
}
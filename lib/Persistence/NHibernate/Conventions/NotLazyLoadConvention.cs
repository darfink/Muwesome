using System;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace Muwesome.Persistence.NHibernate.Conventions {
  internal class NotLazyLoadConvention :
      IReferenceConvention,
      IHasManyConvention,
      IClassConventionAcceptance,
      IClassConvention {
    /// <inheritdoc />
    public void Apply(IManyToOneInstance instance) => instance.Not.LazyLoad();

    /// <inheritdoc />
    public void Apply(IOneToManyCollectionInstance instance) => instance.Not.LazyLoad();

    /// <inheritdoc />
    public void Accept(IAcceptanceCriteria<IClassInspector> critera) {
      // Only mark entity class instances as lazy loadable, configuration
      // objects must remain lazy loadable to enable the 2nd level cache.
      critera.Expect(x => this.IsEntity(x.EntityType));
    }

    /// <inheritdoc />
    public void Apply(IClassInstance instance) => instance.Not.LazyLoad();

    private bool IsEntity(Type type) =>
      type.Namespace.StartsWith(typeof(Muwesome.DomainModel.Entities.Account).Namespace);
  }
}
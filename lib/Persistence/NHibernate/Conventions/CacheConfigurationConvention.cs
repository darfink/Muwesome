using System;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace Muwesome.Persistence.NHibernate.Conventions {
  internal class CacheConfigurationConvention :
      IHasManyConventionAcceptance,
      IHasManyConvention,
      IClassConventionAcceptance,
      IClassConvention {
    /// <summary>The name of the configuration cache region.</summary>
    public static readonly string RegionName = "Configuration";

    /// <inheritdoc />
    public void Accept(IAcceptanceCriteria<IOneToManyCollectionInspector> critera) =>
      critera.Expect(x => this.IsConfiguration(x.EntityType));

    /// <inheritdoc />
    public void Apply(IOneToManyCollectionInstance instance) {
      // TODO: Change to readonly if write-behind 2nd level caching becomes possible
      instance.Cache.IncludeAll();
      instance.Cache.ReadWrite();
      instance.Cache.Region(RegionName);
    }

    /// <inheritdoc />
    public void Accept(IAcceptanceCriteria<IClassInspector> critera) =>
      critera.Expect(x => this.IsConfiguration(x.EntityType));

    /// <inheritdoc />
    public void Apply(IClassInstance instance) {
      instance.Cache.IncludeAll();
      instance.Cache.ReadWrite();
      instance.Cache.Region(RegionName);
    }

    private bool IsConfiguration(Type type) =>
      type.Namespace.StartsWith(typeof(Muwesome.DomainModel.Configuration.GameConfiguration).Namespace);
  }
}
using System;

namespace Muwesome.DomainModel {
  public abstract class Entity {
    /// <summary>Gets or sets the ID.</summary>
    public virtual Guid Id { get; set; }
  }
}
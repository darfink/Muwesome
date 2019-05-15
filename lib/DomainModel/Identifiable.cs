using System;

namespace Muwesome.DomainModel {
  /// <summary>Represents an identifiable instance.</summary>
  public abstract class Identifiable {
    /// <summary>Gets or sets the ID.</summary>
    public virtual Guid Id { get; set; }
  }
}
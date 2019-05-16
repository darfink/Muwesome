using System;

namespace Muwesome.DomainModel {
  /// <summary>Represents an identifiable instance.</summary>
  public abstract class Identifiable {
    /// <summary>Gets or sets the ID.</summary>
    public virtual Guid Id { get; set; }

    /// <inheritdoc />
    public override bool Equals(object other) =>
      other is Identifiable identifiable && identifiable.Id == this.Id;

    /// <inheritdoc />
    public override int GetHashCode() => this.Id.GetHashCode();
  }
}
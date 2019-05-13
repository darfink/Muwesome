using System;

namespace Muwesome.DomainModel {
  public abstract class Identifiable {
    /// <summary>Gets or sets the ID.</summary>
    public Guid Id { get; set; }
  }
}
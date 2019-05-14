using Muwesome.DomainModel.Components;

namespace Muwesome.DomainModel.Configuration {
  public class MapGate : Entity {
    /// <summary>Gets or sets the area of the gate.</summary>
    public virtual Rectangle Area { get; set; }
  }
}
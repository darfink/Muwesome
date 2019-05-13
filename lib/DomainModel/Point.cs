namespace Muwesome.DomainModel {
  /// <summary>Represents a position in 2D space.</summary>
  // TODO: This should be a struct but is required to be a class due to EF restrictions (see: https://github.com/aspnet/EntityFrameworkCore/issues/9906)
  public class Point {
    /// <summary>Gets or sets the horizontal position</summary>
    public byte X { get; set; }

    /// <summary>Gets or sets the vertical position</summary>
    public byte Y { get; set; }
  }
}
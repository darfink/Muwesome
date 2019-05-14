namespace Muwesome.DomainModel.Components {
  public struct Rectangle {
    public byte Top { get; set; }

    public byte Left { get; set; }

    public byte Right => (byte)(this.Left + this.Width);

    public byte Bottom => (byte)(this.Top + this.Height);

    public byte Width { get; set; }

    public byte Height { get; set; }

    public bool Contains(Point point) =>
      point.X >= this.Left && point.X < this.Right &&
      point.Y >= this.Top && point.Y < this.Bottom;
  }
}
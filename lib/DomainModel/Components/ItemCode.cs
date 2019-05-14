namespace Muwesome.DomainModel.Components {
  public struct ItemCode {
    private const int ItemsPerGroup = 512;

    public short Code { get; set; }

    public byte Group => (byte)(this.Code / ItemsPerGroup);

    public byte Number => (byte)(this.Code % ItemsPerGroup);
  }
}

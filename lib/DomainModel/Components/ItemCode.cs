namespace Muwesome.DomainModel.Components {
  /// <summary>Represents an item definition identifier.</summary>
  public struct ItemCode {
    /// <summary>The number of items per group.</summary>
    private const int ItemsPerGroup = 512;

    public ItemCode(byte group, byte number) {
      this.Code = checked((short)((group * ItemsPerGroup) + number));
    }

    public ItemCode(short code) => this.Code = code;

    /// <summary>Gets or sets the code.</summary>
    public short Code { get; set; }

    /// <summary>Gets the item group.</summary>
    public byte Group => (byte)(this.Code / ItemsPerGroup);

    /// <summary>Gets the item index within the group.</summary>
    public byte Number => (byte)(this.Code % ItemsPerGroup);
  }
}
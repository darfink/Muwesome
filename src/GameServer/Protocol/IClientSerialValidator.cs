namespace Muwesome.GameServer.Protocol {
  /// <summary>Represents an object that validates client serials.</summary>
  internal interface IClientSerialValidator {
    /// <summary>Gets or sets a value indicating whether client serials are validated or not.</summary>
    bool ValidateClientSerial { get; set; }
  }
}
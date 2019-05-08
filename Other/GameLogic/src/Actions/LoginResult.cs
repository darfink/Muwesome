namespace Muwesome.GameLogic.Actions {
  /// <summary>Represents all possible login results.</summary>
  public enum LoginResult : byte {
    /// <summary>The password is invalid.</summary>
    InvalidPassword = 0,
    /// <summary>The login was successful.</summary>
    Success = 1,
    /// <summary>The account is invalid.</summary>
    InvalidAccount = 2,
    /// <summary>The account is already connected.</summary>
    AccountIsAlreadyConnected = 3,
    /// <summary>The server is full.</summary>
    ServerIsFull = 4,
    /// <summary>The account is blocked.</summary>
    AccountIsBlocked = 5,
    /// <summary>The game version is invalid.</summary>
    InvalidGameVersion = 6,
    /// <summary>Too many failed login attempts.</summary>
    TooManyFailedLoginAttempts = 8,
    /// <summary>Missing payment information.</summary>
    NoPaymentInformation = 9,
    /// <summary>Subscription term is over.</summary>
    SubscriptionIsOver = 10,
    /// <summary>Subscription term is over for IP address.</summary>
    SubscriptionIsOverForIpAddress = 13,
    /// <summary>The age is below the requirement.</summary>
    IneligibleAge = 17,
    /// <summary>No remaining play time for the current date.</summary>
    NoPointsForDate = 192,
    /// <summary>No remaining play time.</summary>
    NoPoints = 193,
    /// <summary>The IP address is banned.</summary>
    IpAddressIsBanned = 194,
    /// <summary>An internal error.</summary>
    InternalError = 255,
  }
}
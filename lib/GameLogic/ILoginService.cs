using System.Threading.Tasks;
using Muwesome.GameLogic.Actions.Players;

namespace Muwesome.GameLogic {
  /// <summary>Represents a login service.</summary>
  public interface ILoginService {
    /// <summary>Attempts to login an account using the provided credentials.</summary>
    Task<LoginResult> TryLoginAsync(string username, string password);

    /// <summary>Attempts to log out an account by its username.</summary>
    Task<bool> TryLogoutAsync(string username);
  }
}
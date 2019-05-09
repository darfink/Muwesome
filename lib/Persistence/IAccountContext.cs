using System.Threading.Tasks;
using Muwesome.DomainModel.Entities;

namespace Muwesome.Persistence {
  /// <summary>An interface for account contexts.</summary>
  public interface IAccountContext : IContext {
    /// <summary>Gets an account by its credentials.</summary>
    Task<(Account account, bool validPassword)> GetAccountByCredentialsAsync(string username, string password);
  }
}
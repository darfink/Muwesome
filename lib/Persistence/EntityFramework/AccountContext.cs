using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Muwesome.DomainModel.Entities;
using static BCrypt.Net.BCrypt;

namespace Muwesome.Persistence.EntityFramework {
  /// <summary>An context for accounts.</summary>
  internal class AccountContext : PersistenceContext, IAccountContext {
    /// <summary>Initializes a new instance of the <see cref="AccountContext"/> class.</summary>
    public AccountContext(DbContext context)
        : base(context) {
    }

    /// <inheritdoc />
    public async Task<(Account, bool)> GetAccountByCredentialsAsync(string username, string password) {
      var account = await this.Context.Set<Account>().FirstOrDefaultAsync(a => a.Username == username);

      if (account != null) {
        return (account, Verify(password, account.PasswordHash));
      }

      return (null, false);
    }
  }
}
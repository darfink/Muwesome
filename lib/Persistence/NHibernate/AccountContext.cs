using System.Linq;
using System.Threading.Tasks;
using Muwesome.DomainModel.Entities;
using NHibernate;
using NHibernate.Linq;
using static BCrypt.Net.BCrypt;

namespace Muwesome.Persistence.NHibernate {
  /// <summary>An context for accounts.</summary>
  internal class AccountContext : PersistenceContext, IAccountContext {
    /// <summary>Initializes a new instance of the <see cref="AccountContext"/> class.</summary>
    public AccountContext(ISession session)
        : base(session) {
    }

    /// <inheritdoc />
    public Task<Account> GetAccountByUsernameAsync(string username) {
      using (this.WithConnection()) {
        return this.Session.Query<Account>().FirstOrDefaultAsync(a => a.Username == username);
      }
    }

    /// <inheritdoc />
    public async Task<(Account, bool)> GetAccountByCredentialsAsync(string username, string password) {
      Account account = await this.GetAccountByUsernameAsync(username);

      if (account != null) {
        return (account, Verify(password, account.PasswordHash));
      }

      return (null, false);
    }
  }
}
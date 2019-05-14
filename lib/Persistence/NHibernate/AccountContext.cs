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
    public async Task<(Account, bool)> GetAccountByCredentialsAsync(string username, string password) {
      Account account = null;
      using (this.Connect()) {
        account = await this.Session.Query<Account>().FirstOrDefaultAsync(a => a.Username == username);
      }

      if (account != null) {
        return (account, Verify(password, account.PasswordHash));
      }

      return (null, false);
    }
  }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Muwesome.DomainModel.Entities;
using Muwesome.Persistence;

namespace Muwesome.LoginServer {
  internal class AccountController {
    private readonly object readLock = new object();
    private readonly Dictionary<Guid, Account> connectedAccounts = new Dictionary<Guid, Account>();
    private readonly IPersistenceContextProvider persistenceContextProvider;
    private readonly Configuration config;

    /// <summary>Initializes a new instance of the <see cref="AccountController"/> class.</summary>
    public AccountController(Configuration config, IPersistenceContextProvider persistenceContextProvider) {
      this.persistenceContextProvider = persistenceContextProvider;
      this.config = config;
    }

    /// <summary>Gets the number of accounts connected.</summary>
    public int AccountsConnected {
      get {
        lock (this.readLock) {
          return this.connectedAccounts.Count;
        }
      }
    }

    /// <summary>Attempts to login an account using the provided credentials.</summary>
    public async Task<AccountLoginResult> LoginAccountAsync(string username, string password) {
      var accountLoginResult = new AccountLoginResult();
      using (var accountContext = this.persistenceContextProvider.CreateAccountContext()) {
        // TODO: Alternative way to return this information?
        var (account, validPassword) = await accountContext.GetAccountByCredentialsAsync(username, password);

        if (account == null) {
          accountLoginResult.Type = AccountLoginResultType.InvalidAccount;
        } else if (this.IsAccountTimedOut(account)) {
          accountLoginResult.Type = AccountLoginResultType.LockedOut;
        } else if (validPassword) {
          accountLoginResult.Type = AccountLoginResultType.InvalidPassword;
          account.ConsecutiveFailedLoginAttempts++;
          account.LastFailedLoginTime = DateTime.UtcNow;
          await accountContext.SaveChangesAsync();
        } else if (!this.TryAddAccount(account)) {
          accountLoginResult.Type = AccountLoginResultType.AlreadyConnected;
        } else {
          accountLoginResult.Type = AccountLoginResultType.Success;
          accountLoginResult.Account = account;
          account.ConsecutiveFailedLoginAttempts = 0;
          account.LastFailedLoginTime = null;
          account.LastLoginTime = DateTime.UtcNow;
          await accountContext.SaveChangesAsync();
        }
      }

      return accountLoginResult;
    }

    /// <summary>Attempts to log out an account by its ID.</summary>
    public Task<bool> LogoutAccountAsync(Guid accountId) {
      lock (this.readLock) {
        return Task.FromResult(this.connectedAccounts.Remove(accountId));
      }
    }

    /// <summary>Attempts to log out a collection of account IDs.</summary>
    public Task<int> LogoutAccountsAsync(IEnumerable<Guid> accountIds) {
      int successfulLogouts = 0;
      lock (this.readLock) {
        foreach (var accountId in accountIds) {
          if (this.connectedAccounts.Remove(accountId)) {
            successfulLogouts++;
          }
        }

        return Task.FromResult(successfulLogouts);
      }
    }

    private bool IsAccountTimedOut(Account account) {
      if (account.LastFailedLoginTime == null) {
        return false;
      }

      var attempts = Math.Max(account.ConsecutiveFailedLoginAttempts - this.config.FailedLoginAttemptsUntilAccountLockOut, 0);
      var delay = TimeSpan.FromSeconds(Math.Min(Math.Pow(2, attempts), this.config.MaxAccountLockOut.Seconds));

      return delay.TotalSeconds > 0 && (DateTime.UtcNow - account.LastFailedLoginTime.Value) <= delay;
    }

    private bool TryAddAccount(Account account) {
      lock (this.readLock) {
        if (this.connectedAccounts.ContainsKey(account.Id)) {
          return false;
        }

        this.connectedAccounts.Add(account.Id, account);
        return true;
      }
    }
  }
}
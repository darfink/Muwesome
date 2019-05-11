using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Muwesome.Common;
using Muwesome.DomainModel.Entities;
using Muwesome.Persistence;

namespace Muwesome.LoginServer {
  public sealed class LoginServer : LifecycleController, IAccountLoginService {
    private readonly object readLock = new object();
    private readonly Dictionary<string, Account> activeAccounts = new Dictionary<string, Account>();
    private readonly IPersistenceContextProvider persistenceContextProvider;
    private readonly Configuration config;

    /// <summary>Initializes a new instance of the <see cref="LoginServer"/> class.</summary>
    internal LoginServer(Configuration config, IPersistenceContextProvider persistenceContextProvider) {
      this.persistenceContextProvider = persistenceContextProvider;
      this.config = config;
    }

    /// <inheritdoc />
    public int AccountsLoggedIn {
      get {
        lock (this.readLock) {
          return this.activeAccounts.Count;
        }
      }
    }

    /// <inheritdoc />
    public async Task<LoginError?> TryLoginAsync(string username, string password) {
      using (var accountContext = this.persistenceContextProvider.CreateAccountContext()) {
        // TODO: Alternative way to return this information?
        var (account, validPassword) = await accountContext.GetAccountByCredentialsAsync(username, password);

        if (account == null) {
          return LoginError.InvalidAccount;
        } else if (this.IsAccountTimedOut(account)) {
          return LoginError.AccountIsLockedOut;
        } else if (validPassword) {
          account.ConsecutiveFailedLoginAttempts++;
          account.LastFailedLoginTime = DateTime.UtcNow;
          await accountContext.SaveChangesAsync();
          return LoginError.InvalidPassword;
        } else if (!this.TryAddAccount(account)) {
          return LoginError.AccountIsAlreadyConnected;
        } else {
          account.ConsecutiveFailedLoginAttempts = 0;
          account.LastFailedLoginTime = null;
          account.LastLoginTime = DateTime.UtcNow;
          await accountContext.SaveChangesAsync();
          return null;
        }
      }
    }

    /// <inheritdoc />
    public Task<bool> TryLogoutAsync(string username) {
      lock (this.readLock) {
        return Task.FromResult(this.activeAccounts.Remove(username));
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
        if (this.activeAccounts.ContainsKey(account.Username)) {
          return false;
        }

        this.activeAccounts.Add(account.Username, account);
        return true;
      }
    }
  }
}
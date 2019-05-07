using System;
using System.Threading;

namespace Muwesome.ConnectServer.Utility {
  internal static class ReaderWriterLockSlimExtensions {
    /// <summary>Returns a scope guard for an upgradeable read lock.</summary>
    public static IDisposable UpgradeableReadLock(this ReaderWriterLockSlim rwl) {
      rwl.EnterUpgradeableReadLock();
      return new ScopeGuard<ReaderWriterLockSlim>(rwl, cx => cx.ExitUpgradeableReadLock());
    }

    /// <summary>Returns a scope guard for a write lock.</summary>
    public static IDisposable WriteLock(this ReaderWriterLockSlim rwl) {
      rwl.EnterWriteLock();
      return new ScopeGuard<ReaderWriterLockSlim>(rwl, cx => cx.ExitWriteLock());
    }
  }
}
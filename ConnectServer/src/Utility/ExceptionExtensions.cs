using System;

namespace Muwesome.ConnectServer.Utility {
  internal static class ExceptionExtensions {
    /// <summary>Gets an exception by type if it exists in the error chain.</summary>
    public static T GetExceptionByType<T>(this Exception ex) where T : Exception {
      for (; ex != null; ex = ex.InnerException) {
        T target = ex as T;

        if (target != null) {
          return target;
        }
      }

      return null;
    }
  }
}
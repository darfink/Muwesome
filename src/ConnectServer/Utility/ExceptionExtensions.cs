using System;

namespace Muwesome.ConnectServer.Utility {
  internal static class ExceptionExtensions {
    /// <summary>Gets an exception by type if it exists in the error chain.</summary>
    public static T GetExceptionByType<T>(this Exception ex)
        where T : Exception {
      for (; ex != null; ex = ex.InnerException) {
        if (ex is T target) {
          return target;
        }
      }

      return null;
    }
  }
}
using System.Collections.Generic;

namespace Muwesome.ConnectServer.Utility {
  public static class CollectionExtensions {
    /// <summary>Returns a read-only wrapper for the collection.</summary>
    public static ReadOnlyCollectionWrapper<T> AsReadOnly<T>(this ICollection<T> collection) =>
      new ReadOnlyCollectionWrapper<T>(collection);
  }
}
using System.Collections;
using System.Collections.Generic;

namespace Muwesome.ConnectServer {
  public class ReadOnlyCollectionWrapper<T> : IReadOnlyCollection<T> {
    private readonly ICollection<T> _collection;

    /// <summary>Constructs a new read only collection adapter.</summary>
    public ReadOnlyCollectionWrapper(ICollection<T> collection) => _collection = collection;

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => _collection.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => _collection.GetEnumerator();

    /// <inheritdoc />
    public int Count => _collection.Count;
  }

  public static class CollectionExtensions {
    /// <summary>Returns a read-only wrapper for the collection.</summary>
    public static ReadOnlyCollectionWrapper<T> AsReadOnly<T>(this ICollection<T> collection) =>
      new ReadOnlyCollectionWrapper<T>(collection);
  }
}
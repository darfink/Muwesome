using System.Collections;
using System.Collections.Generic;

namespace Muwesome.ConnectServer.Utility {
  internal class ReadOnlyCollectionWrapper<T> : IReadOnlyCollection<T> {
    private readonly ICollection<T> collection;

    /// <summary>Initializes a new instance of the <see cref="ReadOnlyCollectionWrapper{T}"/> class.</summary>
    public ReadOnlyCollectionWrapper(ICollection<T> collection) => this.collection = collection;

    /// <inheritdoc />
    public int Count => this.collection.Count;

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => this.collection.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => this.collection.GetEnumerator();
  }
}
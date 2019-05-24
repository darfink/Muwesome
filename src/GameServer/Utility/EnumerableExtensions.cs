using System.Collections.Generic;
using System.Linq;

namespace Muwesome.GameServer.Utility {
  internal static class EnumerableExtensions {
    public static IEnumerable<(int Index, T Item)> Enumerate<T>(this IEnumerable<T> enumerable) =>
      enumerable.Select((item, index) => (index, item));
  }
}
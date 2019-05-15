using System;
using System.Collections.Generic;

namespace Muwesome.Persistence.Initialization.Utility {
  public static class ListExtensions {
    public static void AddRange<T>(this IList<T> list, IEnumerable<T> items) {
      if (list is List<T> listImpl) {
        listImpl.AddRange(items);
      } else {
        foreach (var item in items) {
          list.Add(item);
        }
      }
    }
  }
}
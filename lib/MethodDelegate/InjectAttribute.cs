using System;

namespace Muwesome.MethodDelegate {
  /// <summary>Marks a parameter as injected.</summary>
  [AttributeUsage(AttributeTargets.Parameter)]
  public class InjectAttribute : Attribute {
  }
}
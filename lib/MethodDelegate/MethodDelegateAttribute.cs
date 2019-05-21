using System;

namespace Muwesome.MethodDelegate {
  /// <summary>Marks a method as a delegate implementor.</summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class MethodDelegateAttribute : Attribute {
    /// <summary>Initializes a new instance of the <see cref="MethodDelegateAttribute"/> class.</summary>
    public MethodDelegateAttribute(Type targetDelegate) {
      if (!typeof(Delegate).IsAssignableFrom(targetDelegate)) {
        throw new ArgumentException($"Target must be a {nameof(Delegate)} type", nameof(targetDelegate));
      }

      this.Target = targetDelegate;
    }

    /// <summary>Gets the delegate target type.</summary>
    public Type Target { get; }
  }
}
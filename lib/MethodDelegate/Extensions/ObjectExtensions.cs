using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Muwesome.MethodDelegate.Extensions {
  public static class ObjectExtensions {
    /// <summary>Gets an instance's method delegate by type parameter.</summary>
    public static Delegate GetMethodDelegate<T>(this object instance, ResolveParameter resolveParameter = null) =>
      MethodDelegateHelper.GetMethodDelegate<T>(instance, resolveParameter);

    /// <summary>Gets an instance's method delegate by type.</summary>
    public static Delegate GetMethodDelegate(this object instance, Type delegateType, ResolveParameter resolveParameter = null) =>
      MethodDelegateHelper.GetMethodDelegate(instance, delegateType, resolveParameter);

    /// <summary>Gets an instance's method delegates.</summary>
    public static IEnumerable<Delegate> GetMethodDelegates(this object instance, ResolveParameter resolveParameter = null) =>
      MethodDelegateHelper.GetMethodDelegates(instance, resolveParameter);
  }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Muwesome.MethodDelegate.Extensions {
  public static class ObjectExtensions {
    /// <summary>Gets an instance's method delegate by type parameter.</summary>
    public static Delegate GetMethodDelegate<T>(this object instance, ParameterResolver parameterResolver = null, bool includeNonPublicMethods = false) =>
      MethodDelegateHelper.GetMethodDelegate<T>(instance, parameterResolver, includeNonPublicMethods);

    /// <summary>Gets an instance's method delegate by type.</summary>
    public static Delegate GetMethodDelegate(this object instance, Type delegateType, ParameterResolver parameterResolver = null, bool includeNonPublicMethods = false) =>
      MethodDelegateHelper.GetMethodDelegate(instance, delegateType, parameterResolver, includeNonPublicMethods);

    /// <summary>Gets an instance's method delegates.</summary>
    public static IEnumerable<Delegate> GetMethodDelegates(this object instance, ParameterResolver parameterResolver = null, bool includeNonPublicMethods = false) =>
      MethodDelegateHelper.GetMethodDelegates(instance, parameterResolver, includeNonPublicMethods);
  }
}
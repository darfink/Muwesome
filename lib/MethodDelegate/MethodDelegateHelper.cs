using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Muwesome.MethodDelegate {
  /// <summary>Returns an instance for an injected parameter.</summary>
  public delegate object ParameterResolver(ParameterInfo parameter);

  public static class MethodDelegateHelper {
    /// <summary>Gets a class method delegate by type parameter.</summary>
    public static Delegate GetMethodDelegate<T>(Type methodClassType, ParameterResolver parameterResolver = null, bool includeNonPublicMethods = false) =>
      GetMethodDelegate(methodClassType, typeof(T), parameterResolver, includeNonPublicMethods);

    /// <summary>Gets a class method delegate by type.</summary>
    public static Delegate GetMethodDelegate(Type methodClassType, Type delegateType, ParameterResolver parameterResolver = null, bool includeNonPublicMethods = false) =>
      DefaultBuilder(methodClassType, parameterResolver, includeNonPublicMethods)
        .Filter((_, @delegate) => @delegate == delegateType)
        .Build()
        .FirstOrDefault();

    /// <summary>Gets an instance's method delegate by type parameter.</summary>
    public static Delegate GetMethodDelegate<T>(object instance, ParameterResolver parameterResolver = null, bool includeNonPublicMethods = false) =>
      GetMethodDelegate(instance, typeof(T), parameterResolver, includeNonPublicMethods);

    /// <summary>Gets an instance's method delegate by type.</summary>
    public static Delegate GetMethodDelegate(object instance, Type delegateType, ParameterResolver parameterResolver = null, bool includeNonPublicMethods = false) =>
      DefaultBuilder(instance.GetType(), parameterResolver, includeNonPublicMethods)
        .MethodClassInstance(instance)
        .AllowMethodBinding(BindingFlags.Instance)
        .Filter((_, @delegate) => @delegate == delegateType)
        .Build()
        .FirstOrDefault();

    /// <summary>Gets an instance's method delegates.</summary>
    public static IEnumerable<Delegate> GetMethodDelegates(object instance, ParameterResolver parameterResolver = null, bool includeNonPublicMethods = false) =>
      DefaultBuilder(instance.GetType(), parameterResolver, includeNonPublicMethods)
        .MethodClassInstance(instance)
        .AllowMethodBinding(BindingFlags.Instance)
        .Build();

    /// <summary>Gets the default method delegate builder.</summary>
    private static MethodDelegateBuilder DefaultBuilder(Type methodClassType, ParameterResolver parameterResolver, bool includeNonPublicMethods) =>
      new MethodDelegateBuilder(methodClassType)
        .ParameterResolver(parameterResolver)
        .AllowMethodBinding(includeNonPublicMethods ? BindingFlags.NonPublic : 0);
  }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Muwesome.MethodDelegate {
  /// <summary>Returns an instance for an injected parameter.</summary>
  public delegate object ResolveParameter(Type parameter);

  public static class MethodDelegateHelper {
    /// <summary>Gets an instance's method delegate by type parameter.</summary>
    public static Delegate GetMethodDelegate<T>(object instance, ResolveParameter resolveParameter = null) =>
      GetMethodDelegate(instance, typeof(T), resolveParameter);

    /// <summary>Gets an instance's method delegate by type.</summary>
    public static Delegate GetMethodDelegate(object instance, Type delegateType, ResolveParameter resolveParameter = null) =>
      GetInstanceMethodDelegatePairs(instance)
        .Where(p => p.Delegate == delegateType)
        .Select(p => CreateDelegateFromMethod(instance, p.Method, p.Delegate, resolveParameter))
        .FirstOrDefault();

    /// <summary>Gets an instance's method delegates.</summary>
    public static IEnumerable<Delegate> GetMethodDelegates(object instance, ResolveParameter resolveParameter = null) =>
      GetInstanceMethodDelegatePairs(instance)
        .Select(p => CreateDelegateFromMethod(instance, p.Method, p.Delegate, resolveParameter));

    /// <summary>Gets all method delegate pairs from an instance.</summary>
    private static IEnumerable<(MethodInfo Method, MethodInfo Delegate)> GetInstanceMethodDelegatePairs(object instance) {
      foreach (var method in instance.GetType().GetMethods()) {
        if (method.GetCustomAttributes(typeof(MethodDelegateAttribute), true).FirstOrDefault() is MethodDelegateAttribute methodDelegate) {
          yield return (method, methodDelegate.Target.GetMethod("Invoke"));
        }
      }
    }

    /// <summary>Creates a delegate from a method delegate.</summary>
    private static Delegate CreateDelegateFromMethod(object instance, MethodInfo method, MethodInfo @delegate, ResolveParameter resolveParameter) {
      if (method.ReturnType != @delegate.ReturnType) {
        throw new Exception($"Method {method.Name} return type differ from delegate {@delegate.DeclaringType.Name}");
      }

      var methodDelegateParameters = method.GetParameters();
      var targetDelegateParameters = @delegate.GetParameters();

      var parametersToForward = methodDelegateParameters.Where(p => !IsInjectedParameter(p));
      Func<ParameterInfo, Type> parameterType = parameter => parameter.ParameterType;

      if (!parametersToForward.Select(parameterType).SequenceEqual(targetDelegateParameters.Select(parameterType))) {
        throw new Exception($"Method {method.Name} parameters differ from delegate {@delegate.DeclaringType.Name}");
      }

      var methodParameters = methodDelegateParameters.Select(GetExpressionForParameter).ToArray();
      var callMethod = Expression.Call(Expression.Constant(instance), method, methodParameters);

      var delegateParameters = methodParameters.OfType<ParameterExpression>();
      return Expression.Lambda(@delegate.DeclaringType, callMethod, nameof(CreateDelegateFromMethod), delegateParameters).Compile();

      Expression GetExpressionForParameter(ParameterInfo parameter) {
        return IsInjectedParameter(parameter)
          ? (Expression)Expression.Constant(resolveParameter(parameter.ParameterType))
          : (Expression)Expression.Parameter(parameter.ParameterType, parameter.Name);
      }
    }

    /// <summary>Returns whether a parameter is injected or not.</summary>
    private static bool IsInjectedParameter(ParameterInfo parameter) => parameter.GetCustomAttribute(typeof(InjectAttribute)) != null;
  }
}
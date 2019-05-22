using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Muwesome.MethodDelegate {
  internal delegate bool MethodDelegateFilter(MethodInfo method, MethodInfo @delegate);

  internal class MethodDelegateBuilder {
    private const BindingFlags DefaultBindingFlags = BindingFlags.Static | BindingFlags.Public;

    private readonly Type methodClass;
    private BindingFlags methodBindingFlags;
    private MethodDelegateFilter methodDelegateFilter;
    private ParameterResolver parameterResolver;
    private object methodClassInstance;

    public MethodDelegateBuilder(Type methodClass) {
      this.methodClass = methodClass;
      this.methodBindingFlags = DefaultBindingFlags;
    }

    public MethodDelegateBuilder AllowMethodBinding(BindingFlags methodBindingFlags) {
      this.methodBindingFlags |= methodBindingFlags;
      return this;
    }

    public MethodDelegateBuilder MethodClassInstance(object methodClassInstance) {
      this.methodClassInstance = methodClassInstance;
      return this;
    }

    public MethodDelegateBuilder ParameterResolver(ParameterResolver parameterResolver) {
      this.parameterResolver = parameterResolver;
      return this;
    }

    public MethodDelegateBuilder Filter(MethodDelegateFilter methodDelegateFilter) {
      this.methodDelegateFilter = methodDelegateFilter;
      return this;
    }

    public IEnumerable<Delegate> Build() {
      foreach (var method in this.methodClass.GetMethods(this.methodBindingFlags)) {
        if (method.GetCustomAttributes(typeof(MethodDelegateAttribute), true).FirstOrDefault() is MethodDelegateAttribute methodDelegate) {
          var @delegate = methodDelegate.Target.GetMethod("Invoke");

          if (this.methodDelegateFilter == null || this.methodDelegateFilter(method, @delegate)) {
            yield return this.CreateDelegateFromMethod(method, @delegate);
          }
        }
      }
    }

    /// <summary>Creates a delegate from a method delegate.</summary>
    private Delegate CreateDelegateFromMethod(MethodInfo method, MethodInfo @delegate) {
      if (method.ReturnType != @delegate.ReturnType) {
        throw new Exception($"Method {method.Name} return type differ from delegate {@delegate.DeclaringType.Name}");
      }

      var methodDelegateParameters = method.GetParameters();
      var targetDelegateParameters = @delegate.GetParameters();

      var parametersToForward = methodDelegateParameters.Where(p => !this.IsInjectedParameter(p));
      Func<ParameterInfo, Type> parameterType = parameter => parameter.ParameterType;

      if (!parametersToForward.Select(parameterType).SequenceEqual(targetDelegateParameters.Select(parameterType))) {
        throw new Exception($"Method {method.Name} parameters differ from delegate {@delegate.DeclaringType.Name}");
      }

      var methodParameters = methodDelegateParameters.Select(GetExpressionForParameter).ToArray();
      var callMethod = this.CreateMethodCall(method, methodParameters);

      var delegateParameters = methodParameters.OfType<ParameterExpression>();
      return Expression.Lambda(@delegate.DeclaringType, callMethod, nameof(this.CreateDelegateFromMethod), delegateParameters).Compile();

      Expression GetExpressionForParameter(ParameterInfo parameter) {
        return this.IsInjectedParameter(parameter)
          ? (Expression)Expression.Constant(this.parameterResolver.Invoke(parameter))
          : (Expression)Expression.Parameter(parameter.ParameterType, parameter.Name);
      }
    }

    /// <summary>Creates a method call to a static or non-static method.</summary>
    private MethodCallExpression CreateMethodCall(MethodInfo method, IEnumerable<Expression> parameters) {
      if (method.IsStatic) {
        return Expression.Call(method, parameters);
      }

      if (this.methodClassInstance == null) {
        throw new ArgumentException($"An instance must be provided for the non-static method {method.Name}");
      }

      return Expression.Call(Expression.Constant(this.methodClassInstance), method, parameters);
    }

    /// <summary>Returns whether a parameter is injected or not.</summary>
    private bool IsInjectedParameter(ParameterInfo parameter) => parameter.GetCustomAttribute(typeof(InjectAttribute)) != null;
  }
}
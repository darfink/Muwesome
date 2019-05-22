# MethodDelegate

A library that enables methods to implement delegates.

It supports (among other) the following features:
- Parameter injection
- Instance methods
- Static methods

### Example

```csharp
using MethodDelegate;
using MethodDelegate.Extensions;

public class DelegateExample {
  public delegate float Foo(int x);

  public delegate void Bar(int x);

  [MethodDelegate(typeof(Foo))]
  public float FooMethod([Inject] int a, int x) => a + x;

  [MethodDelegate(typeof(Foo))]
  public static void BarMethod(int x) => x + 10;

  public static void Main(string[] args) {
    var foo = new DelegateExample().GetMethodDelegate<Foo>(_ => 5);
    System.Console.WriteLine(foo(10)); // 15

    var bar = MethodDelegateHelper.GetMethodDelegate<Bar>(typeof(DelegateExample));
    System.Console.WriteLine(bar(10)); // 20
  }
}
```
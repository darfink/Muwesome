using System;
using System.IO;
using System.Reflection;

namespace Muwesome.Persistence.Initialization {
  public static class ResourceManager {
    private static readonly string Root = string.Join(".", typeof(ResourceManager).Namespace, "Resources");
    private static readonly Assembly ThisAssembly = Assembly.GetAssembly(typeof(ResourceManager));

    /// <summary>Gets a stream fron an embedded resource.</summary>
    public static Stream GetResourceStream(string name) =>
      ThisAssembly.GetManifestResourceStream(string.Join(".", Root, name));
  }
}
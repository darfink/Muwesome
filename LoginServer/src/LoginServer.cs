using System;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using Muwesome.ServerCommon;

namespace Muwesome.LoginServer {
  public class LoginServer : LifecycleServerBase {
    /// <summary>Initializes a new instance of the <see cref="LoginServer"/> class.</summary>
    public LoginServer(params ILifecycle[] lifecycleInstances)
        : base(lifecycleInstances) {
    }
  }
}
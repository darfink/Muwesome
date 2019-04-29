using System;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using Muwesome.ServerCommon;

namespace Muwesome.LoginServer {
  public class LoginServer : LifecycleServerBase {
    public LoginServer(params ILifecycle[] lifecycleInstances)
        : base(lifecycleInstances) {
    }
  }
}
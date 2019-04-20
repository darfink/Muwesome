using System;

namespace Muwesome.ConnectServer {
  // TODO: TRACK DA UPTIME FFS
  // TODO: Cmds, blacklist? exit? actvserv? Over gRPC?
  public class Server {
    public Server(Configuration config) {
      Config = config;
    }

    public Configuration Config { get; }
  }
}
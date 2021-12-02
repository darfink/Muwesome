<div align="center">

# `Muwesome`

## Open-source implementation of Mu Online (season 1.02c)

[![License][license-shield]][license]

 </div>

An experimental server implementation of Mu Online, consisting of multiple
microservices communicating over gRPC orchestrated via Docker. The client
protocol has been reverse-engineered using IDA Pro and Wireshark. The database
layer is implemented using NHibernate and houses a 2nd level cache for all
configuration objects.

# Architecture

The server consists of multiple services, each being dedicated to a single task.
The services are containerized and can be orchestrated via Docker Compose.

![Overview](https://i.imgur.com/uoKzIxT.png)

- **ConnectServer**

  The ConnectServer is directly exposed to the internet, and communicates
  directly to the client via its binary protocol (TCP), whilst gRPC is used to
  communicate with the internal microservices. It supplies the client with a
  list of available game servers upon authentication. It may also require the
  client to be updated, in case it's outdated.

- **LoginServer**

  The LoginServer's sole responsibility is to authenticate users and maintain
  their sessions. Other services may query it, to determine whether a user
  session is valid. It also throttles login attempts using exponential back off.

- **GameServer**

  The GameServer represents a single instance of an isolated world. It registers
  itself at the ConnectServer (gRPC) to flag itself as eligible to receive new
  clients. Clients can communicate either directly, or via a reverse-proxy, with
  the GameServer using the client's binary protocol (TCP).

# Improvements

- **ChatServer**

  The client has built-in support for chatting in between different game servers, but the service itself must be implemented.

- **Monitoring**

  Integrating Sentry to allow real-time performance monitoring and error
  tracking.

<!-- Links -->
[license-shield]: https://img.shields.io/crates/l/region.svg?style=for-the-badge
[license]: https://github.com/darfink/region-rs
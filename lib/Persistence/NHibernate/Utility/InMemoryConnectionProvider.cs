using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Connection;
using NHibernate.Driver;

namespace Muwesome.Persistence.NHibernate.Utility {
  /// <summary>A connection provider for a persistent in-memory database.</summary>
  public class InMemoryConnectionProvider : IConnectionProvider {
    private readonly SQLiteConnection connection = new SQLiteConnection("Data Source=:memory:");

    /// <summary>Initializes a new instance of the <see cref="InMemoryConnectionProvider"/> class.</summary>
    public InMemoryConnectionProvider() => this.connection.Open();

    /// <inheritdoc />
    public IDriver Driver => new SQLite20Driver();

    /// <inheritdoc />
    public void Configure(IDictionary<string, string> settings) {
    }

    /// <inheritdoc />
    public void CloseConnection(DbConnection onnection) {
    }

    /// <inheritdoc />
    public DbConnection GetConnection() => this.connection;

    /// <inheritdoc />
    public Task<DbConnection> GetConnectionAsync(CancellationToken cancellationToken) => Task.FromResult(this.connection as DbConnection);

    /// <inheritdoc />
    public void Dispose() => this.connection?.Dispose();
  }
}
using System.Data.Common;

namespace Utility.Database
{
  public interface IDbConnectionInfo
  {
    /// <summary>
    /// Set the connection string name.
    /// 
    /// When set, loads connection information ConfigurationManager.ConnectionStrings
    /// as typically specified in a web.config or app.config file
    /// </summary>
    string ConnectionStringName { set; }

    /// <summary>
    /// Get or set the full connection string
    /// </summary>
    string ConnectionString { get; set; }

    /// <summary>
    /// The provider factory used to create connections.
    /// 
    /// Valid for connection types supporting provider factories.
    /// 
    /// This can be either a registered provider name such as:
    ///   System.Data.SqlClient
    ///   Npgsql
    /// or the type of the provider's factory as an assembly qualified name such as:
    ///   System.Data.SqlClient.SqlClientFactory, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    ///   Npgsql.NpgsqlFactory, Npgsql
    /// </summary>
    string Provider { get; set; }

    /// <summary>
    /// Creates and returns a provider factory instance based on the valud of the Provider attribute.
    /// 
    /// Valid for connection types supporting provider factories.
    /// </summary>
    DbProviderFactory ProviderFactory { get; } 

    string ServerAddress { get; }
    int? ServerPort { get; }
    string DatabaseName { get; }
    string UserName { get; }
    string Password { get; }

    /// <summary>
    /// Makes a copy of this instance
    /// </summary>
    /// <returns>A copy of this instance</returns>
    IDbConnectionInfo Copy();
  }
}
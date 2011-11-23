using System.Data.Common;

namespace Utility.Database
{
  public interface IDbConnectionInfo
  {
    string Name { get; set; }
    string ConnectionString { get; set; }

    /// <summary>
    /// The provider used to create connections.  This can be either a registered
    /// provider name such as:
    ///   System.Data.SqlClient
    ///   Npgsql
    /// or the type of the provider's factory as an assembly qualified name such as:
    ///   System.Data.SqlClient.SqlClientFactory, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    ///   Npgsql.NpgsqlFactory, Npgsql
    /// </summary>
    string Provider { get; set; }

    DbProviderFactory ProviderFactory { get; }
  }
}

using System.Data.Common;

namespace Utility.Database
{
  public interface IDbConnection
  {
    string ConnectionString { get; }
    string ProviderName { get; }
    DbProviderFactory ProviderFactory { get; }
  }
}
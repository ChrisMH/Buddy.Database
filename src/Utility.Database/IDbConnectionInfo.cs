using System.Data.Common;

namespace Utility.Database
{
  public interface IDbConnectionInfo
  {
    string Name { get; }
    string ConnectionString { get; }
    string ProviderName { get; }
    DbProviderFactory ProviderFactory { get; }
  }
}
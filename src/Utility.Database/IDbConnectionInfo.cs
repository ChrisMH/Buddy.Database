using System.Data.Common;

namespace Utility.Database
{
  public interface IDbConnectionInfo
  {
    string Name { get; set; }
    string ConnectionString { get; set; }
    string ProviderName { get; set; }
    DbProviderFactory ProviderFactory { get; }
  }
}
using System;
using System.Configuration;
using System.Data.Common;

namespace Utility.Database
{
  public class DbConnection : IDbConnection
  {
    public DbConnection()
    {
    }

    public DbConnection(string connectionStringName)
    {
      if (string.IsNullOrEmpty(connectionStringName))
        throw new ArgumentException("Connection string name not provided", "connectionStringName");
      if(ConfigurationManager.ConnectionStrings[connectionStringName] == null)
        throw new ArgumentException(string.Format("Connection string name '{0}' not found in the configuration", connectionStringName), "connectionStringName");

      ConnectionString = new DbConnectionStringBuilder {ConnectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString}.ConnectionString;
      ProviderName = ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName;
    }


    public string ConnectionString { get; set; }
    public DbProviderFactory ProviderFactory { get; set; }

    public string ProviderName
    {
      get { return providerName; }
      set
      {
        if(string.IsNullOrEmpty(value))
        {
          providerName = null;
          ProviderFactory = null;
        }
        else
        {
          providerName = value;
          try
          {
            ProviderFactory = DbProviderFactories.GetFactory(providerName);
          }
          catch (ArgumentException e)
          {
            throw new ArgumentException(string.Format("Could not create a DbProviderFactory from provider name '{0}'", providerName), "ProviderName", e);
          }
        }
      }
    }


    private string providerName;
  }
}
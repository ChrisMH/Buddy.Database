using System;
using System.Configuration;
using System.Data.Common;

namespace Utility.Database
{
  public class DbConnectionInfo : IDbConnectionInfo
  {
    public DbConnectionInfo()
    {
    }

    public DbConnectionInfo(IDbConnectionInfo copy)
    {
      Name = copy.Name;
      ConnectionString = copy.ConnectionString;
      ProviderName = copy.ProviderName; 
    }

    public DbConnectionInfo(string connectionStringName)
    {
      if (string.IsNullOrEmpty(connectionStringName))
        throw new ArgumentException("Connection string name not provided", "connectionStringName");
      if(ConfigurationManager.ConnectionStrings[connectionStringName] == null)
        throw new ArgumentException(string.Format("Connection string name '{0}' not found in the configuration", connectionStringName), "connectionStringName");

      Name = connectionStringName;
      ConnectionString = new DbConnectionStringBuilder {ConnectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString}.ConnectionString;
      ProviderName = ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName;
      if(string.IsNullOrEmpty(ProviderName))
      {
        ProviderName = null;
      }
    }

    public string Name { get; set; }
    public string ConnectionString { get; set; }
    public string ProviderName { get; set; }
    public DbProviderFactory ProviderFactory
    {
      get
      {
        if (string.IsNullOrEmpty(ProviderName))
        {
          return null;
        }

        try
        {
          return DbProviderFactories.GetFactory(ProviderName);
        }
        catch (ArgumentException e)
        {
          throw new ArgumentException(string.Format("Could not create a DbProviderFactory from provider name '{0}'", ProviderName), "ProviderName", e);
        }
      }
    }
  }
}
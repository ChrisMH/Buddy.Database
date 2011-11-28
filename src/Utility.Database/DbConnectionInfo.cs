using System;
using System.Configuration;
using System.Data.Common;
using System.Reflection;

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
      Provider = copy.Provider;
    }

    public DbConnectionInfo(string connectionStringName)
    {
      if (string.IsNullOrEmpty(connectionStringName))
        throw new ArgumentException("Connection string name not provided", "connectionStringName");
      if (ConfigurationManager.ConnectionStrings[connectionStringName] == null)
        throw new ArgumentException(string.Format("Connection string name '{0}' not found in the configuration", connectionStringName), "connectionStringName");

      Name = connectionStringName;
      ConnectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
      Provider = ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName;
      if (string.IsNullOrEmpty(Provider))
      {
        Provider = null;
      }
    }

    public string Name { get; set; }
    public string ConnectionString { get; set; }
    public string Provider { get; set; }

    public DbProviderFactory ProviderFactory
    {
      get
      {
        if (string.IsNullOrEmpty(Provider))
        {
          return null;
        }

        try
        {
          // First try to create from a registered provider name
          return DbProviderFactories.GetFactory(Provider);
        }
        catch (ArgumentException)
        {
          // If that fails, try to create it by type
          try
          {
            var providerType = new ReflectionType(Provider).CreateType();
            var instanceField = providerType.GetField("Instance", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public);
            if (instanceField == null && !instanceField.FieldType.IsSubclassOf(typeof (DbProviderFactory)))
            {
              throw new ArgumentException(string.Format("Could not load a provider factory for '{0}'", Provider), "Provider");
            }

            return (DbProviderFactory) instanceField.GetValue(null);
          }
          catch (Exception e)
          {
            if (e is ArgumentException) throw;
            throw new ArgumentException(string.Format("Could not load a provider factory for '{0}'", Provider), "Provider");
          }
        }
      }
    }
  }
}
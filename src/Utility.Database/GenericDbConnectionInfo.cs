using System;
using System.Configuration;
using System.Data.Common;
using System.Reflection;

namespace Utility.Database
{
  /// <summary>
  /// Generic implementation assuming that the connection is provider based and 
  /// the connection string consists of key-value pairs that can be decoded using DbConnectionStringBuilder
  /// </summary>
  public class GenericDbConnectionInfo : IDbConnectionInfo, IDbProviderInfo
  {
    public GenericDbConnectionInfo()
    {
    }

    public IDbConnectionInfo Copy()
    {
      var copy = new GenericDbConnectionInfo();

      Name = copy.Name;
      ConnectionString = copy.ConnectionString;
      if(copy is IDbProviderInfo)
      {
        Provider = ((IDbProviderInfo) copy).Provider;
      }
    }

    public GenericDbConnectionInfo(string connectionStringName)
    {
      if (string.IsNullOrWhiteSpace(connectionStringName))
        throw new ArgumentException("Connection string name not provided", "connectionStringName");
      if (ConfigurationManager.ConnectionStrings[connectionStringName] == null)
        throw new ArgumentException(string.Format("Connection string name '{0}' not found in the configuration", connectionStringName), "connectionStringName");

      Name = connectionStringName;
      connectionString = new DbConnectionStringBuilder { ConnectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString };
      Provider = ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName;
      if (string.IsNullOrEmpty(Provider))
      {
        Provider = null;
      }
    }

    public string Name { get; set; }
    public string ConnectionString
    {
      get { return connectionString == null ? null : connectionString.ConnectionString; }
      set { connectionString = new DbConnectionStringBuilder {ConnectionString = value}; }
    }
    
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

    public object this[string key]
    {
      get { return new DbConnectionStringBuilder {ConnectionString = ConnectionString}[key]; }
    }

    private DbConnectionStringBuilder connectionString;
  }
}
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
      connectionString = new DbConnectionStringBuilder();
    }

    public string ConnectionStringName
    {
      get { return connectionStringName; }
      set
      {
        if (string.IsNullOrWhiteSpace(value))
          throw new ArgumentException("Connection string name not provided", "ConnectionStringName");
        if (ConfigurationManager.ConnectionStrings[value] == null)
          throw new ArgumentException(string.Format("Connection string name '{0}' not found in the configuration", connectionStringName), "ConnectionStringName");

        connectionStringName = value;
        connectionString = new DbConnectionStringBuilder {ConnectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString};
        Provider = ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName;
      }
    }

    public string ConnectionString
    {
      get { return connectionString.ConnectionString; }
      set { connectionString = value == null ? new DbConnectionStringBuilder() : new DbConnectionStringBuilder {ConnectionString = value}; }
    }
    
    public string Provider { get; set; }

    public DbProviderFactory ProviderFactory
    {
      get
      {
        if (string.IsNullOrWhiteSpace(Provider))
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

    public virtual string ServerAddress { get { return null; } }
    public virtual int? ServerPort { get { return null; } }
    public virtual string DatabaseName { get { return null; } }
    public virtual string UserName { get { return null; } }
    public virtual string Password { get { return null; } }

    public virtual IDbConnectionInfo Copy()
    {
      var copy = new GenericDbConnectionInfo();
      InternalCopy(copy);
      return copy;
    }

    protected virtual void InternalCopy(GenericDbConnectionInfo copy)
    {
      copy.connectionStringName = connectionStringName;
      copy.connectionString = connectionString == null ? null : new DbConnectionStringBuilder {ConnectionString = connectionString.ConnectionString};
      copy.Provider = Provider;
    }

    private string connectionStringName;
    protected DbConnectionStringBuilder connectionString;
  }
}
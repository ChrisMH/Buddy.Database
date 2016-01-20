using System;
using System.Configuration;
using System.Data.Common;
using System.Reflection;

namespace Utility.Database
{
  /// <summary>
  ///   Generic implementation assuming that the connection is provider based and the connection string consists of key-value pairs that can be decoded using DbConnectionStringBuilder
  /// </summary>
  public class DbConnectionInfo : IDbConnectionInfo
  {
    public virtual string ConnectionStringName
    {
      set
      {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("ConnectionStringName not provided", "ConnectionStringName");
        if (ConfigurationManager.ConnectionStrings[value] == null)
          throw new ArgumentException(string.Format("ConnectionStringName '{0}' not found in the configuration", value), "ConnectionStringName");

        ConnectionString = ConfigurationManager.ConnectionStrings[value].ConnectionString;
        if(!string.IsNullOrWhiteSpace(ConfigurationManager.ConnectionStrings[value].ProviderName))
        {
          Provider = ConfigurationManager.ConnectionStrings[value].ProviderName;
        }
      }
    }

    public virtual string ConnectionString
    {
      get { return connectionString; }
      set
      {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("ConnectionString not provided", "ConnectionString");
        connectionString = value;
      }
    }

    public virtual string Provider { get; set; }

    public virtual DbProviderFactory ProviderFactory
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

    public virtual string ServerAddress
    {
      get { return null; }
    }

    public virtual int? ServerPort
    {
      get { return null; }
    }

    public virtual string DatabaseName
    {
      get { return null; }
    }

    public virtual string UserName
    {
      get { return null; }
    }

    public virtual string Password
    {
      get { return null; }
    }

    public IDbConnectionInfo Copy()
    {
      var copy = (IDbConnectionInfo) GetType().Assembly.CreateInstance(GetType().FullName);
      if(connectionString != null) copy.ConnectionString = connectionString;
      if(Provider != null) copy.Provider = Provider;
      return copy;
    }

    private string connectionString;
  }
}
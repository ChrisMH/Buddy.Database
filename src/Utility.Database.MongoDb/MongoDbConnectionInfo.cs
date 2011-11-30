using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Utility.Database.MongoDb
{
  /// <summary>
  /// Connection string information for MongoDb
  /// 
  /// Acceptable values for the indexer:
  /// ServerAddress
  /// ServerPort
  /// DatabaseName
  /// UserName
  /// Password
  /// </summary>
  public class MongoDbConnectionInfo : IDbConnectionInfo
  {
    internal const string ServerAddressKey = "ServerAddress";
    internal const string ServerPortKey = "ServerPort";
    internal const string DatabaseNameKey = "DatabaseName";
    internal const string UserNameKey = "UserName";
    internal const string PasswordKey = "Password";

    private const string ConnectionStringRegex = @"mongodb://((?<UserName>[^:]+):(?<Password>[^@]+)@)?(?<ServerAddress>[^/:]+)(:(?<ServerPort>[^/]+))?/(?<DatabaseName>[^/]+)/?";
    
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
        ConnectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
      }
    }

    public string ConnectionString
    {
      get { return connectionString; }
      set
      {
        if(string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Connection string not provided", "ConnectionString");

        var match = Regex.Match(value, ConnectionStringRegex);
        if (!match.Success) throw new ArgumentException(string.Format("Invalid connection string format: '{0}'", value), "ConnectionString");
          
        connectionString = value;
        connectionStringParts.Clear();

        if(match.Groups[ServerAddressKey].Success) connectionStringParts[ServerAddressKey] = match.Groups[ServerAddressKey].Value;
        if(match.Groups[ServerPortKey].Success) connectionStringParts[ServerPortKey] = match.Groups[ServerPortKey].Value;
        if(match.Groups[DatabaseNameKey].Success) connectionStringParts[DatabaseNameKey] = match.Groups[DatabaseNameKey].Value;
        if(match.Groups[UserNameKey].Success) connectionStringParts[UserNameKey] = match.Groups[UserNameKey].Value;
        if(match.Groups[PasswordKey].Success) connectionStringParts[PasswordKey] = match.Groups[PasswordKey].Value;
        
      }
    }

    public bool ContainsKey(string key)
    {
      return connectionStringParts.ContainsKey(key);
    }

    public object this[string key]
    {
      get { return connectionStringParts.ContainsKey(key) ? connectionStringParts[key] : null; }
    }

    public IDbConnectionInfo Copy()
    {
      return new MongoDbConnectionInfo
                 {
                   connectionStringName = connectionStringName,
                   ConnectionString = ConnectionString
                 };
    }

    private string connectionStringName;
    private string connectionString;
    private readonly Dictionary<string, object> connectionStringParts = new Dictionary<string, object>();
  }
}
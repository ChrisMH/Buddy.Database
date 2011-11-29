using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Utility;

namespace Utility.Database.Management.MongoDb
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
    private const string ConnectionStringRegex = @"mongodb://((?<UserName>[^:]+):(?<Password>[^@]+)@)?(?<ServerAddress>[^/:]+)(:(?<ServerPort>[^/]+))?/(?<DatabaseName>[^/]+)/?";

    public MongoDbConnectionInfo()
    {
    }

    public MongoDbConnectionInfo(IDbConnectionInfo copy)
    {
      Name = copy.Name;
      ConnectionString = copy.ConnectionString;
    }

    public MongoDbConnectionInfo(string connectionStringName)
    {
      if (string.IsNullOrWhiteSpace(connectionStringName))
        throw new ArgumentException("Connection string name not provided", "connectionStringName");
      if (ConfigurationManager.ConnectionStrings[connectionStringName] == null)
        throw new ArgumentException(string.Format("Connection string name '{0}' not found in the configuration", connectionStringName), "connectionStringName");

      Name = connectionStringName;
      ConnectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
    }

    public string Name { get; set; }

    public string ConnectionString
    {
      get { return connectionString; }
      set
      {
        connectionString = value;
        connectionStringParts.Clear();
        if(!string.IsNullOrWhiteSpace(connectionString))
        {
          var match = Regex.Match(connectionString, ConnectionStringRegex);
          if(!match.Success) throw new ArgumentException(string.Format("Invalid connection string format: '{0}'", connectionString), "connectionString");

          var enumerator = match.Groups.GetEnumerator();
          while(enumerator.MoveNext())
          {
            Debug.WriteLine(enumerator.Current.ToString());
          }
        }
      }
    }

    public object this[string key]
    {
      get { throw new System.NotImplementedException(); }
    }

    private string connectionString;
    private Dictionary<string, object> connectionStringParts = new Dictionary<string, object>();
  }
}
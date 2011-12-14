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
  internal sealed class MongoDbConnectionInfo : DbConnectionInfo
  {
    internal const string ServerAddressKey = "ServerAddress";
    internal const string ServerPortKey = "ServerPort";
    internal const string DatabaseNameKey = "DatabaseName";
    internal const string UserNameKey = "UserName";
    internal const string PasswordKey = "Password";

    private const string ConnectionStringRegex = @"mongodb://((?<UserName>[^:]+):(?<Password>[^@]+)@)?(?<ServerAddress>[^/:]+)(:(?<ServerPort>[^/]+))?/(?<DatabaseName>[^/]+)$";
    
    public override string ConnectionString
    {
      set
      {
        base.ConnectionString = value;

        var match = Regex.Match(base.ConnectionString, ConnectionStringRegex);
        if (!match.Success) throw new ArgumentException(string.Format("Invalid connection string format: '{0}'", value), "ConnectionString");
          
        connectionStringParts.Clear();

        if(match.Groups[ServerAddressKey].Success) connectionStringParts[ServerAddressKey] = match.Groups[ServerAddressKey].Value;
        if(match.Groups[ServerPortKey].Success) connectionStringParts[ServerPortKey] = Convert.ToInt32(match.Groups[ServerPortKey].Value);
        if(match.Groups[DatabaseNameKey].Success) connectionStringParts[DatabaseNameKey] = match.Groups[DatabaseNameKey].Value;
        if(match.Groups[UserNameKey].Success) connectionStringParts[UserNameKey] = match.Groups[UserNameKey].Value;
        if(match.Groups[PasswordKey].Success) connectionStringParts[PasswordKey] = match.Groups[PasswordKey].Value;
      }
    }

    public override string ServerAddress
    {
      get { return (string)connectionStringParts[ServerAddressKey]; }
    }

    public override int? ServerPort
    {
      get { return connectionStringParts.ContainsKey(ServerPortKey) ? (int?)connectionStringParts[ServerPortKey] : null; }
    }

    public override string DatabaseName
    {
      get { return (string)connectionStringParts[DatabaseNameKey]; }
    }

    public override string UserName
    {
      get { return connectionStringParts.ContainsKey(UserNameKey) ? (string)connectionStringParts[UserNameKey] : null; }
    }

    public override string Password
    {
      get { return connectionStringParts.ContainsKey(PasswordKey) ? (string)connectionStringParts[PasswordKey] : null; }
    }

    private readonly Dictionary<string, object> connectionStringParts = new Dictionary<string, object>();
  }
}
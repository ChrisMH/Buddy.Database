using System;
using System.Data.Common;

namespace Utility.Database.RavenDb
{
  /// <summary>
  /// Connection string information for RavenDb
  /// </summary>
  internal sealed class RavenDbConnectionInfo : DbConnectionInfo
  {
    internal const string ServerAddressKey = "Url";
    internal const string DatabaseNameKey = "Database";
    internal const string UserNameKey = "User";
    internal const string PasswordKey = "Password";

    public override string ConnectionString
    {
      set
      {
        try
        {
          base.ConnectionString = value;
          connectionStringBuilder = new DbConnectionStringBuilder { ConnectionString = base.ConnectionString };
        }
        catch (Exception e)
        {
          throw new ArgumentException(string.Format("Could not parse connection string : {0}", value), "ConnectionString", e);
        }
      }
    }

    public override string ServerAddress
    {
      get { return connectionStringBuilder.ContainsKey(ServerAddressKey) ? (string)connectionStringBuilder[ServerAddressKey] : null;}
    }


    public override string DatabaseName
    {
      get { return connectionStringBuilder.ContainsKey(DatabaseNameKey) ? (string)connectionStringBuilder[DatabaseNameKey] : null; }
    }

    public override string UserName
    {
      get { return connectionStringBuilder.ContainsKey(UserNameKey) ? (string)connectionStringBuilder[UserNameKey] : null; }
    }

    public override string Password
    {
      get { return connectionStringBuilder.ContainsKey(PasswordKey) ? (string)connectionStringBuilder[PasswordKey] : null; }
    }

    private DbConnectionStringBuilder connectionStringBuilder = new DbConnectionStringBuilder();
  }
}
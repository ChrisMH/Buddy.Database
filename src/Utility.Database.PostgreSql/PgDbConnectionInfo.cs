using System;
using System.Data.Common;

namespace Utility.Database.PostgreSql
{
  internal sealed class PgDbConnectionInfo : DbConnectionInfo
  {
    internal const string ServerAddressKey = "host";
    internal const string ServerPortKey = "port";
    internal const string DatabaseNameKey = "database";
    internal const string UserNameKey = "user id";
    internal const string PasswordKey = "password";
    
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

    public override int? ServerPort
    {
      get { return connectionStringBuilder.ContainsKey(ServerPortKey) ? (int?)Convert.ToInt32(connectionStringBuilder[ServerPortKey]) : null; }
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
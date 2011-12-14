using System;
using System.Data.Common;

namespace Utility.Database.Mock
{
  internal sealed class MockDbConnectionInfo : DbConnectionInfo, IMockDbConnectionInfo
  {
    internal const string DatabaseNameKey = "database";
    internal const string DatabaseTypeKey = "database type";


    public override string ConnectionString
    {
      set
      {
        try
        {
          base.ConnectionString = value;
          connectionStringBuilder = new DbConnectionStringBuilder { ConnectionString = base.ConnectionString };

          if(!connectionStringBuilder.ContainsKey(DatabaseNameKey))
            throw new ArgumentException(string.Format("ConnectionString does not contain required key '{0}'", DatabaseNameKey), "ConnectionString");
          if (!connectionStringBuilder.ContainsKey(DatabaseTypeKey))
            throw new ArgumentException(string.Format("ConnectionString does not contain required key '{0}'", DatabaseTypeKey), "ConnectionString");
        }
        catch (Exception e)
        {
          throw new ArgumentException(string.Format("Could not parse connection string : {0}", value), "ConnectionString", e);
        }
      }
    }


    public override string DatabaseName
    {
      get { return connectionStringBuilder.ContainsKey(DatabaseNameKey) ? (string) connectionStringBuilder[DatabaseNameKey] : null; }
    }

    public Type DatabaseType
    {
      get { return connectionStringBuilder.ContainsKey(DatabaseTypeKey) ? new ReflectionType((string)connectionStringBuilder[DatabaseTypeKey]).CreateType() : null; }
    }

    public override string ServerAddress
    {
      get { throw new NotImplementedException(); }
    }

    public override int? ServerPort
    {
      get { throw new NotImplementedException(); }
    }

    public override string UserName
    {
      get { throw new NotImplementedException(); }
    }

    public override string Password
    {
      get { throw new NotImplementedException(); }
    }

    private DbConnectionStringBuilder connectionStringBuilder = new DbConnectionStringBuilder();
  }
}
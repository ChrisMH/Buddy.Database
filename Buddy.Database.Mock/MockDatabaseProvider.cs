using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.Database.Mock
{
  public static class MockDatabaseProvider
  {
    /// <summary>
    /// Opens an existing database
    /// </summary>
    /// <param name="connectionInfo"></param>
    /// <returns></returns>
    public static IMockDatabase Open(IDbConnectionInfo connectionInfo)
    {
      connectionInfo = VerifyConnectionInfo(connectionInfo);

      var databaseType = ((IMockDbConnectionInfo) connectionInfo).DatabaseType;

      if (!mockDatabases.ContainsKey(databaseType)) throw new ArgumentException("Database does not exist", "connectionInfo");
      if (!mockDatabases[databaseType].ContainsKey(connectionInfo.DatabaseName)) throw new ArgumentException("Database does not exist", "connectionInfo");

      return mockDatabases[databaseType][connectionInfo.DatabaseName];
    }

    /// <summary>
    /// Creates if the database does not exist, otherwise opens
    /// </summary>
    /// <param name="connectionInfo"></param>
    /// <returns></returns>
    public static IMockDatabase Create(IDbConnectionInfo connectionInfo)
    {
      connectionInfo = VerifyConnectionInfo(connectionInfo);

      var databaseType = ((IMockDbConnectionInfo)connectionInfo).DatabaseType;

      if (!mockDatabases.ContainsKey(databaseType))
      {
        // Check that the type is OK
        var interfaces = databaseType.FindInterfaces((type, criteria) => type == typeof(IMockDatabase), null);
        if (interfaces.Length == 0) throw new ArgumentException(string.Format("Mock database type does not implement IMockDatabase : {0}", databaseType), "connectionInfo.DatabaseType");

        if (databaseType.GetConstructor(Type.EmptyTypes) == null) throw new ArgumentException(string.Format("Mock database type does not have a parameterless constructor : {0}", databaseType), "connectionInfo.DatabaseType");

        mockDatabases.Add(databaseType, new Dictionary<string, IMockDatabase>());
      }

      if (!mockDatabases[databaseType].ContainsKey(connectionInfo.DatabaseName))
      {
        var ctor = databaseType.GetConstructor(Type.EmptyTypes);
        mockDatabases[databaseType].Add(connectionInfo.DatabaseName, (IMockDatabase)ctor.Invoke(null));
      }

      return mockDatabases[databaseType][connectionInfo.DatabaseName];
    }

    /// <summary>
    /// Destroys the database if it exists
    /// </summary>
    /// <param name="connectionInfo"></param>
    /// <returns></returns>
    public static void Destroy(IDbConnectionInfo connectionInfo)
    {
      connectionInfo = VerifyConnectionInfo(connectionInfo);

      var databaseType = ((IMockDbConnectionInfo)connectionInfo).DatabaseType;

      if(!mockDatabases.ContainsKey(databaseType)) return;

      mockDatabases[databaseType].Remove(connectionInfo.DatabaseName);

      if(mockDatabases[databaseType].Count == 0)
      {
        mockDatabases.Remove(databaseType);
      }
    }

    private static IDbConnectionInfo VerifyConnectionInfo(IDbConnectionInfo connectionInfo)
    {
      if (connectionInfo == null) throw new ArgumentException("connectionInfo not provided", "connectionInfo");

      return new MockDbConnectionInfo {ConnectionString = connectionInfo.ConnectionString};
    }


    internal static void Reset()
    {
      mockDatabases = new Dictionary<Type, Dictionary<string, IMockDatabase>>();
    }

    private static Dictionary<Type,Dictionary<string,IMockDatabase>> mockDatabases = new Dictionary<Type, Dictionary<string, IMockDatabase>>();
  }
}

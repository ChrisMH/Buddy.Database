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
      ValidateConnectionInfo(connectionInfo);

      var databaseType = ((IDbMockTypeInfo) connectionInfo).MockDatabaseType;

      if (!mockDatabases.ContainsKey(databaseType)) throw new ArgumentException("Database does not exist", "connectionInfo");
      if (!mockDatabases[databaseType].ContainsKey(connectionInfo.ConnectionStringName)) throw new ArgumentException("Database does not exist", "connectionInfo");

      return mockDatabases[databaseType][connectionInfo.ConnectionStringName];
    }

    /// <summary>
    /// Creates if the database does not exist, otherwise opens
    /// </summary>
    /// <param name="connectionInfo"></param>
    /// <returns></returns>
    public static IMockDatabase Create(IDbConnectionInfo connectionInfo)
    {
      ValidateConnectionInfo(connectionInfo);

      var databaseType = ((IDbMockTypeInfo)connectionInfo).MockDatabaseType;

      if (!mockDatabases.ContainsKey(databaseType))
      {
        // Check that the type is OK
        var interfaces = databaseType.FindInterfaces((type, criteria) => type == typeof(IMockDatabase), null);
        if (interfaces.Length == 0) throw new ArgumentException(string.Format("Mock database type does not implement IMockDatabase : {0}", databaseType), "connectionInfo.MockDatabaseType");

        if (databaseType.GetConstructor(Type.EmptyTypes) == null) throw new ArgumentException(string.Format("Mock database type does not have a parameterless constructor : {0}", databaseType), "connectionInfo.MockDatabaseType");

        mockDatabases.Add(databaseType, new Dictionary<string, IMockDatabase>());
      }

      if (!mockDatabases[databaseType].ContainsKey(connectionInfo.ConnectionStringName))
      {
        var ctor = databaseType.GetConstructor(Type.EmptyTypes);
        mockDatabases[databaseType].Add(connectionInfo.ConnectionStringName, (IMockDatabase)ctor.Invoke(null));
      }

      return mockDatabases[databaseType][connectionInfo.ConnectionStringName];
    }

    /// <summary>
    /// Destroys the database if it exists
    /// </summary>
    /// <param name="connectionInfo"></param>
    /// <returns></returns>
    public static void Destroy(IDbConnectionInfo connectionInfo)
    {
      ValidateConnectionInfo(connectionInfo);

      var databaseType = ((IDbMockTypeInfo)connectionInfo).MockDatabaseType;

      if(!mockDatabases.ContainsKey(databaseType)) return;

      mockDatabases[databaseType].Remove(connectionInfo.ConnectionStringName);

      if(mockDatabases[databaseType].Count == 0)
      {
        mockDatabases.Remove(databaseType);
      }
    }

    private static void ValidateConnectionInfo(IDbConnectionInfo connectionInfo)
    {
      if (connectionInfo == null) throw new ArgumentException("connectionInfo not provided", "connectionInfo");

      var mockTypeInfo = connectionInfo as IDbMockTypeInfo;
      if (mockTypeInfo == null) throw new ArgumentException("Invalid connection info, does not implement IDbMockTypeInfo", "connectionInfo");

      if (string.IsNullOrEmpty(connectionInfo.ConnectionStringName)) throw new ArgumentException("connectionInfo.ConnectionStringName not provided", "connectionInfo.ConnectionStringName");

      if (mockTypeInfo.MockDatabaseType == null) throw new ArgumentException("connectionInfo.MockDatabaseType not provided", "connectionInfo.MockDatabaseType");
    }


    internal static void Reset()
    {
      mockDatabases = new Dictionary<Type, Dictionary<string, IMockDatabase>>();
    }

    private static Dictionary<Type,Dictionary<string,IMockDatabase>> mockDatabases = new Dictionary<Type, Dictionary<string, IMockDatabase>>();
  }
}

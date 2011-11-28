using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using NUnit.Framework;
using Utility.Database.Management.MongoDb;
using Utility.Logging;

namespace Utility.Database.MongoDb.Test
{
  [SetUpFixture]
  public class GlobalTest
  {
    public static ILogger Logger { get; private set; }
    public static IDbConnectionInfo ConnectionInfo1 { get; private set; }
    public static IDbConnectionInfo ConnectionInfo2 { get; private set; }

    [SetUpAttribute]
    public void SetUp()
    {
      try
      {
        ConnectionInfo1 = new DbConnectionInfo("Test1");
        ConnectionInfo2 = new DbConnectionInfo("Test2");
      }
      catch (Exception e)
      {
        if (Logger != null) Logger.Fatal(e, "SetUp : {0} : {1}", e.GetType(), e.Message);
        throw;
      } 
    }

    [TearDown]
    public void TearDown()
    {
      try
      {
        DropTestDatabaseAndRole();
      }
      catch (Exception e)
      {
        if (Logger != null) Logger.Fatal(e, "TearDown : {0} : {1}", e.GetType(), e.Message);
        throw;
      } 
    }

    public static void DropTestDatabaseAndRole()
    {
      var connectionParams = MongoDbManager.ParseConnectionString(ConnectionInfo1);

      var server = MongoServer.Create(connectionParams.ConnectionString);
      server.DropDatabase(connectionParams.DatabaseName);

      connectionParams = MongoDbManager.ParseConnectionString(ConnectionInfo2);

      server = MongoServer.Create(connectionParams.ConnectionString);
      server.DropDatabase(connectionParams.DatabaseName);

    }
  }
}

using System;
using MongoDB.Driver;
using NUnit.Framework;
using Utility.Logging;
using Utility.Logging.NLog;

namespace Utility.Database.MongoDb.Test
{
  [SetUpFixture]
  public class GlobalTest
  {
    public static ILogger Logger { get; private set; }
    public static MongoDbManager DbManager1 { get; private set; }
    public static MongoDbManager DbManager2 { get; private set; }

    [SetUpAttribute]
    public void SetUp()
    {
      try
      {
        Logger = new NLogLoggerFactory().GetCurrentClassLogger();
   
        DbManager1 = new MongoDbManager
                     {
                       Description = new MongoDbDescription {ConnectionInfo = new MongoDbConnectionInfo {ConnectionStringName = "Test1"}}
                     };

        DbManager2 = new MongoDbManager
                     {
                       Description = new MongoDbDescription {ConnectionInfo = new MongoDbConnectionInfo {ConnectionStringName = "Test1"}}
                     };
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
      DbManager1.CreateServer().DropDatabase(DbManager1.ConnectionInfo.DatabaseName);
      DbManager2.CreateServer().DropDatabase(DbManager2.ConnectionInfo.DatabaseName);
    }
  }
}
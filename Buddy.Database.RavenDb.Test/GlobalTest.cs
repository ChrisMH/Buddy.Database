using System;
using NUnit.Framework;
using Utility.Logging;
using Utility.Logging.NLog;

namespace Utility.Database.RavenDb.Test
{
  [SetUpFixture]
  public class GlobalTest
  {
    public static ILogger Logger { get; private set; }
    public static RavenDbManager DbManager1 { get; private set; }
    public static RavenDbManager DbManager2 { get; private set; }

    [SetUpAttribute]
    public void SetUp()
    {
      try
      {
        Logger = new NLogLoggerFactory().GetCurrentInstanceLogger();
   
        DbManager1 = new RavenDbManager
                     {
                       Description = new DbDescription {ConnectionInfo = new DbConnectionInfo {ConnectionStringName = "Test1"}}
                     };

        DbManager2 = new RavenDbManager
                     {
                       Description = new DbDescription {ConnectionInfo = new DbConnectionInfo {ConnectionStringName = "Test1"}}
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
      //DbManager1.CreateServer().DropDatabase(DbManager1.Description.ConnectionInfo.DatabaseName);
      //DbManager2.CreateServer().DropDatabase(DbManager2.Description.ConnectionInfo.DatabaseName);
    }
  }
}
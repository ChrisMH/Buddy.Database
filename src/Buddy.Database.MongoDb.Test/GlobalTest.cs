using System;
using NUnit.Framework;

namespace Utility.Database.MongoDb.Test
{
  [SetUpFixture]
  public class GlobalTest
  {
    public static MongoDbManager DbManager1 { get; private set; }
    public static MongoDbManager DbManager2 { get; private set; }
    public static NLog.Logger Logger { get; private set; }

    [SetUpAttribute]
    public void SetUp()
    {
      try
      {
        Logger = NLog.LogManager.GetCurrentClassLogger();
   
        DbManager1 = new MongoDbManager
                     {
                       Description = new DbDescription {ConnectionInfo = new DbConnectionInfo {ConnectionStringName = "Test1"}}
                     };

        DbManager2 = new MongoDbManager
                     {
                       Description = new DbDescription {ConnectionInfo = new DbConnectionInfo {ConnectionStringName = "Test1"}}
                     };
      }
      catch (Exception e)
      {
        if (Logger != null) Logger.FatalException(string.Format("SetUp : {0} : {1}", e.GetType(), e.Message), e);
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
        if (Logger != null) Logger.FatalException(string.Format("TearDown : {0} : {1}", e.GetType(), e.Message), e);
        throw;
      }
    }

    public static void DropTestDatabaseAndRole()
    {
      DbManager1.CreateServer().DropDatabase(DbManager1.Description.ConnectionInfo.DatabaseName);
      DbManager2.CreateServer().DropDatabase(DbManager2.Description.ConnectionInfo.DatabaseName);
    }
  }
}
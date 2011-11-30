using System;
using System.Data.Common;
using NUnit.Framework;
using Utility.Logging;
using Utility.Logging.NLog;

namespace Utility.Database.PostgreSql.Test
{
  [SetUpFixture]
  public class GlobalTest
  {
    public static ILogger Logger { get; private set; }
    public static IDbConnectionInfo ConnectionInfo1 { get; private set; }
    public static IDbConnectionInfo ConnectionInfo2 { get; private set; }
    public static PgSuperuser Superuser = new PgSuperuser();
    
    [SetUp]
    public void SetUp()
    {
      try
      {
        Logger = new NLogLoggerFactory().GetCurrentClassLogger();
        ConnectionInfo1 = new GenericDbConnectionInfo {ConnectionStringName = "Test1"};
        var csBuilder = new DbConnectionStringBuilder {ConnectionString = ConnectionInfo1.ConnectionString};
        csBuilder["pooling"] = "false";
        ConnectionInfo1.ConnectionString = csBuilder.ConnectionString;

        ConnectionInfo2 = new GenericDbConnectionInfo {ConnectionStringName = "Test2"};
        csBuilder = new DbConnectionStringBuilder {ConnectionString = ConnectionInfo2.ConnectionString};
        csBuilder["pooling"] = "false";
        ConnectionInfo2.ConnectionString = csBuilder.ConnectionString;
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
      }
      catch (Exception e)
      {
        if (Logger != null) Logger.Fatal(e, "TearDown : {0} : {1}", e.GetType(), e.Message);
        throw;
      }
    }


    public static void DropTestDatabaseAndRole()
    {
      var manager1 = new PgDbManager
      {
        Description = new PgDbDescription { ConnectionInfo = ConnectionInfo1 },
        Superuser = Superuser
        };
        
      var manager2 = new PgDbManager
      {
        Description = new PgDbDescription { ConnectionInfo = ConnectionInfo2 },
        Superuser = Superuser
        };

      using (var conn = manager1.CreateDatabaseConnection())
      {
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandText = string.Format("DROP DATABASE IF EXISTS \"{0}\"", manager1.ConnectionInfo[PgDbManager.DatabaseKey]);
          cmd.ExecuteNonQuery();
        }
      }

      using (var conn = manager2.CreateDatabaseConnection())
      {
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandText = string.Format("DROP DATABASE IF EXISTS \"{0}\"", manager2.ConnectionInfo[PgDbManager.DatabaseKey]);
          cmd.ExecuteNonQuery();
        }
      }


      using (var conn = manager1.CreateDatabaseConnection())
      {
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandText = string.Format("DROP ROLE IF EXISTS \"{0}\"", manager1.ConnectionInfo[PgDbManager.UserIdKey]);
          cmd.ExecuteNonQuery();
        }
      }

      using (var conn = manager2.CreateDatabaseConnection())
      {
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandText = string.Format("DROP ROLE IF EXISTS \"{0}\"", manager2.ConnectionInfo[PgDbManager.UserIdKey]);
          cmd.ExecuteNonQuery();
        }
      }
    }
  }
}
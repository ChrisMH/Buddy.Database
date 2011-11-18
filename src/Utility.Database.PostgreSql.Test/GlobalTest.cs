using System;
using System.Data.Common;
using NUnit.Framework;
using Utility.Database.Management.PostgreSql;
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
      }
      catch (Exception e)
      {
        if (Logger != null) Logger.Fatal(e, "TearDown : {0} : {1}", e.GetType(), e.Message);
        throw;
      }
    }


    public static void DropTestDatabaseAndRole()
    {
      using (var conn = ConnectionInfo1.ProviderFactory.CreateConnection())
      {
        conn.ConnectionString = PgDbManager.CreateDatabaseConnectionString(ConnectionInfo1, Superuser);
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          var csBuilder = new DbConnectionStringBuilder {ConnectionString = ConnectionInfo1.ConnectionString};

          cmd.CommandText = string.Format("DROP DATABASE IF EXISTS \"{0}\"", csBuilder[PgDbManager.DatabaseKey]);
          cmd.ExecuteNonQuery();
        }
      }

      using (var conn = ConnectionInfo2.ProviderFactory.CreateConnection())
      {
        conn.ConnectionString = PgDbManager.CreateDatabaseConnectionString(ConnectionInfo2, Superuser);
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          var csBuilder = new DbConnectionStringBuilder {ConnectionString = ConnectionInfo2.ConnectionString};

          cmd.CommandText = string.Format("DROP DATABASE IF EXISTS \"{0}\"", csBuilder[PgDbManager.DatabaseKey]);
          cmd.ExecuteNonQuery();
        }
      }


      using (var conn = ConnectionInfo1.ProviderFactory.CreateConnection())
      {
        conn.ConnectionString = PgDbManager.CreateDatabaseConnectionString(ConnectionInfo1, Superuser);
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          var csBuilder = new DbConnectionStringBuilder {ConnectionString = ConnectionInfo1.ConnectionString};

          cmd.CommandText = string.Format("DROP ROLE IF EXISTS \"{0}\"", csBuilder[PgDbManager.UserIdKey]);
          cmd.ExecuteNonQuery();
        }
      }

      using (var conn = ConnectionInfo2.ProviderFactory.CreateConnection())
      {
        conn.ConnectionString = PgDbManager.CreateDatabaseConnectionString(ConnectionInfo2, Superuser);
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          var csBuilder = new DbConnectionStringBuilder {ConnectionString = ConnectionInfo2.ConnectionString};

          cmd.CommandText = string.Format("DROP ROLE IF EXISTS \"{0}\"", csBuilder[PgDbManager.UserIdKey]);
          cmd.ExecuteNonQuery();
        }
      }
    }
  }
}
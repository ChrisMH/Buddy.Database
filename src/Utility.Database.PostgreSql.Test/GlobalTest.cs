using System;
using NUnit.Framework;
using Utility.Logging;
using Utility.Logging.NLog;

namespace Utility.Database.PostgreSql.Test
{
  [SetUpFixture]
  public class GlobalTest
  {
    public static ILogger Logger { get; private set; }
    public static PgDbManager Manager1 { get; private set; }
    public static PgDbManager Manager2 { get; private set; }
    public static PgSuperuser Superuser = new PgSuperuser();

    [SetUp]
    public void SetUp()
    {
      try
      {
        Logger = new NLogLoggerFactory().GetCurrentClassLogger();

        Manager1 = new PgDbManager
                   {
                     Description = new PgDbDescription {ConnectionInfo = new PgDbConnectionInfo {ConnectionStringName = "Test1"}},
                     Superuser = Superuser
                   };

        Manager2 = new PgDbManager
                   {
                     Description = new PgDbDescription {ConnectionInfo = new PgDbConnectionInfo {ConnectionStringName = "Test2"}},
                     Superuser = Superuser
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
      }
      catch (Exception e)
      {
        if (Logger != null) Logger.Fatal(e, "TearDown : {0} : {1}", e.GetType(), e.Message);
        throw;
      }
    }


    public static void DropTestDatabaseAndRole()
    {
      using (var conn = Manager1.CreateDatabaseConnection())
      {
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandText = string.Format("DROP DATABASE IF EXISTS \"{0}\"", Manager1.ConnectionInfo.DatabaseName);
          cmd.ExecuteNonQuery();
        }
      }

      using (var conn = Manager2.CreateDatabaseConnection())
      {
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandText = string.Format("DROP DATABASE IF EXISTS \"{0}\"", Manager2.ConnectionInfo.DatabaseName);
          cmd.ExecuteNonQuery();
        }
      }


      using (var conn = Manager1.CreateDatabaseConnection())
      {
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandText = string.Format("DROP ROLE IF EXISTS \"{0}\"", Manager2.ConnectionInfo.UserName);
          cmd.ExecuteNonQuery();
        }
      }

      using (var conn = Manager2.CreateDatabaseConnection())
      {
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandText = string.Format("DROP ROLE IF EXISTS \"{0}\"", Manager2.ConnectionInfo.UserName);
          cmd.ExecuteNonQuery();
        }
      }
    }
  }
}
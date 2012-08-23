using System;
using NUnit.Framework;
using Npgsql;

namespace Utility.Database.PostgreSql.Test
{
  [SetUpFixture]
  public class GlobalTest
  {
    public static PgDbManager Manager1 { get; private set; }
    public static PgDbManager Manager2 { get; private set; }
    public static PgSuperuser Superuser = new PgSuperuser();
    public static NLog.Logger Logger { get; private set; }

    [SetUp]
    public void SetUp()
    {
      try
      {
        Logger = NLog.LogManager.GetCurrentClassLogger();

        Manager1 = new PgDbManager
        {
          Description = new PgDbDescription
          {
            ConnectionInfo = new DbConnectionInfo { ConnectionStringName = "Test1" },
            Superuser = Superuser
          }
        };

        Manager2 = new PgDbManager
        {
          Description = new PgDbDescription
          {
            ConnectionInfo = new DbConnectionInfo { ConnectionStringName = "Test2" },
            Superuser = Superuser
          }
        };
      }
      catch (Exception e)
      {
        if (Logger != null)
          Logger.FatalException(string.Format("SetUp : {0} : {1}", e.GetType(), e.Message), e);
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
        if (Logger != null)
          Logger.FatalException(string.Format("TearDown : {0} : {1}", e.GetType(), e.Message), e);
        throw;
      }
    }

    public static void DropTestDatabaseAndRole()
    {
      NpgsqlConnection.ClearAllPools();
      using (var conn = Manager1.CreateDatabaseConnection())
      {
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandText = string.Format("DROP DATABASE IF EXISTS \"{0}\"", Manager1.Description.ConnectionInfo.DatabaseName);
          cmd.ExecuteNonQuery();
        }
      }

      using (var conn = Manager2.CreateDatabaseConnection())
      {
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandText = string.Format("DROP DATABASE IF EXISTS \"{0}\"", Manager2.Description.ConnectionInfo.DatabaseName);
          cmd.ExecuteNonQuery();
        }
      }

      using (var conn = Manager1.CreateDatabaseConnection())
      {
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandText = string.Format("DROP ROLE IF EXISTS \"{0}\"", Manager2.Description.ConnectionInfo.UserName);
          cmd.ExecuteNonQuery();
        }
      }

      using (var conn = Manager2.CreateDatabaseConnection())
      {
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandText = string.Format("DROP ROLE IF EXISTS \"{0}\"", Manager2.Description.ConnectionInfo.UserName);
          cmd.ExecuteNonQuery();
        }
      }
    }
  }
}
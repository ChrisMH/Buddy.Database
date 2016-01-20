using System;
using System.Data.Common;
using NUnit.Framework;

namespace Utility.Database.PostgreSql.Test
{
  public class PgDbManagerDestroyTest
  {
    [SetUp]
    public void SetUp()
    {
      GlobalTest.DropTestDatabaseAndRole();
    }

    [TearDown]
    public void TearDown()
    {
      GlobalTest.DropTestDatabaseAndRole();
    }

    [Test]
    public void DestroyDropsDatabase()
    {
      var manager = new PgDbManager {Description = new PgDbDescription {ConnectionInfo = GlobalTest.Manager1.Description.ConnectionInfo}};

      manager.Create();
      manager.Destroy();

      using (var conn = manager.CreateDatabaseConnection())
      {
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandText = string.Format("SELECT COUNT(*) FROM pg_catalog.pg_database WHERE datname='{0}'", manager.Description.ConnectionInfo.DatabaseName);
          Assert.AreEqual(0, Convert.ToInt64(cmd.ExecuteScalar()));
        }
      }
    }

    [Test]
    public void DestroyDropsRole()
    {
      var manager = new PgDbManager { Description = new PgDbDescription { ConnectionInfo = GlobalTest.Manager1.Description.ConnectionInfo } };

      manager.Create();
      manager.Destroy();

      using (var conn = manager.CreateDatabaseConnection())
      {
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandText = string.Format("SELECT COUNT(*) FROM pg_catalog.pg_user WHERE usename='{0}'", manager.Description.ConnectionInfo.UserName);
          Assert.AreEqual(0, Convert.ToInt64(cmd.ExecuteScalar()));
        }
      }
    }

    [Test]
    public void DestroyDoesNotDropRoleThatIsStillInUse()
    {
      var manager2 = new PgDbManager { Description = new PgDbDescription { ConnectionInfo = GlobalTest.Manager2.Description.ConnectionInfo } };
      manager2.Create();

      // Create a new manager for connection 1's database with connection 2's user
      var csBuilderT = new DbConnectionStringBuilder { ConnectionString = GlobalTest.Manager1.Description.ConnectionInfo.ConnectionString };
      csBuilderT[PgDbConnectionInfo.UserNameKey] = GlobalTest.Manager2.Description.ConnectionInfo.UserName;
      csBuilderT[PgDbConnectionInfo.PasswordKey] = GlobalTest.Manager2.Description.ConnectionInfo.Password;

      var managerT = new PgDbManager
                     {
                       Description = new PgDbDescription
                                     {
                                       ConnectionInfo = new DbConnectionInfo
                                                        {
                                                          ConnectionString = csBuilderT.ConnectionString,
                                                          Provider = GlobalTest.Manager1.Description.ConnectionInfo.Provider
                                                        }
                                     }
                     };

      managerT.Create();
      managerT.Destroy();

      // connection 2's user should not have been deleted because it is still in use by connection 2's database

      using (var conn = managerT.CreateDatabaseConnection())
      {
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandText = string.Format("SELECT COUNT(*) FROM pg_catalog.pg_user WHERE usename='{0}'", csBuilderT[PgDbConnectionInfo.UserNameKey]);
          Assert.AreEqual(1, Convert.ToInt64(cmd.ExecuteScalar()));
        }
      }
    }
  }
}
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
      var manager = new PgDbManager {Description = new PgDbDescription {ConnectionInfo = GlobalTest.ConnectionInfo1}};

      manager.Create();
      manager.Destroy();

      using (var conn = manager.CreateDatabaseConnection())
      {
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandText = string.Format("SELECT COUNT(*) FROM pg_catalog.pg_database WHERE datname='{0}'", manager.ConnectionInfo[PgDbManager.DatabaseKey]);
          Assert.AreEqual(0, Convert.ToInt64(cmd.ExecuteScalar()));
        }
      }
    }

    [Test]
    public void DestroyDropsRole()
    {
      var manager = new PgDbManager {Description = new PgDbDescription {ConnectionInfo = GlobalTest.ConnectionInfo1}};

      manager.Create();
      manager.Destroy();

      using (var conn = manager.CreateDatabaseConnection())
      {
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandText = string.Format("SELECT COUNT(*) FROM pg_catalog.pg_user WHERE usename='{0}'", manager.ConnectionInfo[PgDbManager.UserIdKey]);
          Assert.AreEqual(0, Convert.ToInt64(cmd.ExecuteScalar()));
        }
      }
    }

    [Test]
    public void DestroyDoesNotDropRoleThatIsStillInUse()
    {
      var manager2 = new PgDbManager {Description = new PgDbDescription {ConnectionInfo = GlobalTest.ConnectionInfo2}};
      manager2.Create();

      // Create a new manager for connection 1's database with connection 2's user
      var csBuilderT = new DbConnectionStringBuilder {ConnectionString = GlobalTest.ConnectionInfo1.ConnectionString};
      csBuilderT[PgDbManager.UserIdKey] = GlobalTest.ConnectionInfo2[PgDbManager.UserIdKey];
      csBuilderT[PgDbManager.PasswordKey] = GlobalTest.ConnectionInfo2[PgDbManager.PasswordKey];

      var managerT = new PgDbManager
                     {
                       Description = new PgDbDescription
                                     {
                                       ConnectionInfo = new GenericDbConnectionInfo
                                                        {
                                                          ConnectionString = csBuilderT.ConnectionString,
                                                          Provider = ((IDbProviderInfo) GlobalTest.ConnectionInfo1).Provider
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
          cmd.CommandText = string.Format("SELECT COUNT(*) FROM pg_catalog.pg_user WHERE usename='{0}'", csBuilderT[PgDbManager.UserIdKey]);
          Assert.AreEqual(1, Convert.ToInt64(cmd.ExecuteScalar()));
        }
      }
    }
  }
}
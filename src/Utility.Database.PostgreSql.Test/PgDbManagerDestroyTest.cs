using System;
using System.Data.Common;
using NUnit.Framework;
using Utility.Database.Management.PostgreSql;

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
      var manager = new PgDbManager(new PgDbDescription {ConnectionInfo = GlobalTest.ConnectionInfo1});
      var csBuilder = new DbConnectionStringBuilder {ConnectionString = GlobalTest.ConnectionInfo1.ConnectionString};

      manager.Create();
      manager.Destroy();

      using (var conn = manager.ConnectionInfo.ProviderFactory.CreateConnection())
      {
        conn.ConnectionString = PgDbManager.CreateDatabaseConnectionString(GlobalTest.ConnectionInfo1, GlobalTest.Superuser);
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandText = string.Format("SELECT COUNT(*) FROM pg_catalog.pg_database WHERE datname='{0}'", csBuilder[PgDbManager.DatabaseKey]);
          Assert.AreEqual(0, Convert.ToInt64(cmd.ExecuteScalar()));
        }
      }
    }

    [Test]
    public void DestroyDropsRole()
    {
      var manager = new PgDbManager(new PgDbDescription {ConnectionInfo = GlobalTest.ConnectionInfo1});
      var csBuilder = new DbConnectionStringBuilder {ConnectionString = GlobalTest.ConnectionInfo1.ConnectionString};

      manager.Create();
      manager.Destroy();

      using (var conn = manager.ConnectionInfo.ProviderFactory.CreateConnection())
      {
        conn.ConnectionString = PgDbManager.CreateDatabaseConnectionString(GlobalTest.ConnectionInfo1, GlobalTest.Superuser);
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandText = string.Format("SELECT COUNT(*) FROM pg_catalog.pg_user WHERE usename='{0}'", csBuilder[PgDbManager.UserIdKey]);
          Assert.AreEqual(0, Convert.ToInt64(cmd.ExecuteScalar()));
        }
      }
    }

    [Test]
    public void DestroyDoesNotDropRoleThatIsStillInUse()
    {
      var manager2 = new PgDbManager(new PgDbDescription {ConnectionInfo = GlobalTest.ConnectionInfo2});
      manager2.Create();

      // Create a new manager for connection 1's database with connection 2's user
      var csBuilder2 = new DbConnectionStringBuilder {ConnectionString = GlobalTest.ConnectionInfo2.ConnectionString};

      var csBuilderT = new DbConnectionStringBuilder {ConnectionString = GlobalTest.ConnectionInfo1.ConnectionString};
      csBuilderT[PgDbManager.UserIdKey] = csBuilder2[PgDbManager.UserIdKey];
      csBuilderT[PgDbManager.PasswordKey] = csBuilder2[PgDbManager.PasswordKey];

      var managerT = new PgDbManager(new PgDbDescription
                                     {
                                       ConnectionInfo = new DbConnectionInfo
                                                        {
                                                          ConnectionString = csBuilderT.ConnectionString,
                                                          ProviderFactory = GlobalTest.ConnectionInfo1.ProviderFactory
                                                        }
                                     });
      
      managerT.Create();
      managerT.Destroy();

      // connection 2's user should not have been deleted because it is still in use by connection 2's database
      
      using (var conn = managerT.ConnectionInfo.ProviderFactory.CreateConnection())
      {
        conn.ConnectionString = PgDbManager.CreateDatabaseConnectionString(managerT.ConnectionInfo, GlobalTest.Superuser);
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
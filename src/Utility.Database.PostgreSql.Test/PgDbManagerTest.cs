using System;
using System.Data.Common;
using NUnit.Framework;

namespace Utility.Database.PostgreSql.Test
{
  public class PgDbManagerTest
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
    public void PgDbManagerCreatedWithNullSuperuserUsesDefaultSuperuser()
    {
      var manager = new PgDbManager {Description = new PgDbDescription {ConnectionInfo = GlobalTest.ConnectionInfo1}};

      Assert.NotNull(manager.Superuser);
      Assert.AreEqual("postgres", manager.Superuser.Database);
      Assert.AreEqual("postgres", manager.Superuser.UserId);
      Assert.AreEqual("postgres", manager.Superuser.Password);
    }

    [Test]
    public void PgDbManagerCreatedWithSuperuserUsesSuperuser()
    {
      var manager = new PgDbManager
                    {
                      Description = new PgDbDescription {ConnectionInfo = GlobalTest.ConnectionInfo1},
                      Superuser = new PgSuperuser {Database = "sudb", UserId = "suid", Password = "supw"}
                    };

      Assert.NotNull(manager.Superuser);
      Assert.AreEqual("sudb", manager.Superuser.Database);
      Assert.AreEqual("suid", manager.Superuser.UserId);
      Assert.AreEqual("supw", manager.Superuser.Password);
    }

    [Test]
    public void CreateDatabaseConnectionStringUsesSuperuserParameters()
    {
      var manager = new PgDbManager
                    {
                      Description = new PgDbDescription {ConnectionInfo = GlobalTest.ConnectionInfo1},
                      Superuser = new PgSuperuser {Database = "sudb", UserId = "suid", Password = "supw"}
                    };
      
      using(var conn = manager.CreateDatabaseConnection())
      {
        var result = new DbConnectionStringBuilder {ConnectionString = conn.ConnectionString};

        Assert.AreEqual("sudb", result[PgDbManager.DatabaseKey]);
        Assert.AreEqual("suid", result[PgDbManager.UserIdKey]);
        Assert.AreEqual("supw", result[PgDbManager.PasswordKey]);
      }
    }

    [Test]
    public void CreateContentConnectionStringUsesSuperuserParameters()
    {
      var manager = new PgDbManager
                    {
                      Description = new PgDbDescription {ConnectionInfo = GlobalTest.ConnectionInfo1},
                      Superuser = new PgSuperuser {Database = "sudb", UserId = "suid", Password = "supw"}
                    };
                    
      using(var conn = manager.CreateContentConnection())
      {
        var result = new DbConnectionStringBuilder {ConnectionString = conn.ConnectionString};

        Assert.AreEqual(manager.ConnectionInfo[PgDbManager.DatabaseKey], result[PgDbManager.DatabaseKey]);
        Assert.AreEqual("suid", result[PgDbManager.UserIdKey]);
        Assert.AreEqual("supw", result[PgDbManager.PasswordKey]);
      }
    }

    [Test]
    public void CreateThrowsWhenParametersAreInvalid()
    {
      InvalidParameterTests(manager => manager.Create());
    }

    [Test]
    public void DestroyThrowsWhenParametersAreInvalid()
    {
      InvalidParameterTests(manager => manager.Destroy());
    }

    [Test]
    public void SeedThrowsWhenParametersAreInvalid()
    {
      InvalidParameterTests(manager => manager.Seed());
    }

    private void InvalidParameterTests(Action<PgDbManager> action)
    {
      var manager = new PgDbManager();
      Assert.AreEqual("Description",
                      Assert.Throws<ArgumentNullException>(() => action.Invoke(manager)).ParamName);
      
      manager = new PgDbManager { Superuser = null };
      Assert.AreEqual("Superuser",
                      Assert.Throws<ArgumentNullException>(() => action.Invoke(manager)).ParamName);

      manager = new PgDbManager {Description = new PgDbDescription()};
      Assert.AreEqual("Description.ConnectionInfo",
                      Assert.Throws<ArgumentNullException>(() => action.Invoke(manager)).ParamName);

      manager = new PgDbManager {Description = new PgDbDescription {ConnectionInfo = new GenericDbConnectionInfo()}};
      Assert.AreEqual("Description.ConnectionInfo.ConnectionString",
                      Assert.Throws<ArgumentException>(() => action.Invoke(manager)).ParamName);

      manager = new PgDbManager {Description = new PgDbDescription {ConnectionInfo = new GenericDbConnectionInfo {ConnectionString = ""}}};
      Assert.AreEqual("Description.ConnectionInfo.ConnectionString",
                      Assert.Throws<ArgumentException>(() => action.Invoke(manager)).ParamName);

      manager = new PgDbManager {Description = new PgDbDescription {ConnectionInfo = new GenericDbConnectionInfo {ConnectionString = "database=database"}}};
      Assert.AreEqual("Description.ConnectionInfo.ProviderFactory",
                      Assert.Throws<ArgumentException>(() => action.Invoke(manager)).ParamName);
    }

    [Test]
    public void CreateFromDescriptionWithNoConnectionThrows()
    {
      var manager = new PgDbManager {Description = new PgDbDescription {XmlRoot = Resources.TestDescriptionNoConnectionName}};

      Assert.AreEqual("Description.ConnectionInfo", Assert.Throws<ArgumentNullException>(manager.Create).ParamName);
    }

    [Test]
    public void SeedFromDescriptionWithNoConnectionThrows()
    {
      var manager = new PgDbManager {Description = new PgDbDescription {XmlRoot = Resources.TestDescriptionNoConnectionName}};

      Assert.AreEqual("Description.ConnectionInfo", Assert.Throws<ArgumentNullException>(manager.Seed).ParamName);
    }

    [Test]
    public void DestroyFromDescriptionWithNoConnectionThrows()
    {
      var manager = new PgDbManager {Description = new PgDbDescription {XmlRoot = Resources.TestDescriptionNoConnectionName}};

      Assert.AreEqual("Description.ConnectionInfo", Assert.Throws<ArgumentNullException>(manager.Destroy).ParamName);
    }
  }
}
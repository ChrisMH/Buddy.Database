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
      var manager = new PgDbManager { Description = new PgDbDescription { ConnectionInfo = GlobalTest.Manager1.Description.ConnectionInfo } };

      Assert.NotNull(manager.Superuser);
      Assert.AreEqual("postgres", manager.Superuser.DatabaseName);
      Assert.AreEqual("postgres", manager.Superuser.UserName);
      Assert.AreEqual("postgres", manager.Superuser.Password);
    }

    [Test]
    public void PgDbManagerCreatedWithSuperuserUsesSuperuser()
    {
      var manager = new PgDbManager
                    {
                      Description = new PgDbDescription { ConnectionInfo = GlobalTest.Manager1.Description.ConnectionInfo },
                      Superuser = new PgSuperuser {DatabaseName = "sudb", UserName = "suid", Password = "supw"}
                    };

      Assert.NotNull(manager.Superuser);
      Assert.AreEqual("sudb", manager.Superuser.DatabaseName);
      Assert.AreEqual("suid", manager.Superuser.UserName);
      Assert.AreEqual("supw", manager.Superuser.Password);
    }

    [Test]
    public void CreateDatabaseConnectionStringUsesSuperuserParameters()
    {
      var manager = new PgDbManager
                    {
                      Description = new PgDbDescription { ConnectionInfo = GlobalTest.Manager1.Description.ConnectionInfo },
                      Superuser = new PgSuperuser {DatabaseName = "sudb", UserName = "suid", Password = "supw"}
                    };
      
      using(var conn = manager.CreateDatabaseConnection())
      {
        var result = new DbConnectionStringBuilder {ConnectionString = conn.ConnectionString};

        Assert.AreEqual("sudb", result[PgDbConnectionInfo.DatabaseNameKey]);
        Assert.AreEqual("suid", result[PgDbConnectionInfo.UserNameKey]);
        Assert.AreEqual("supw", result[PgDbConnectionInfo.PasswordKey]);
      }
    }

    [Test]
    public void CreateContentConnectionStringUsesSuperuserParameters()
    {
      var manager = new PgDbManager
                    {
                      Description = new PgDbDescription { ConnectionInfo = GlobalTest.Manager1.Description.ConnectionInfo },
                      Superuser = new PgSuperuser {DatabaseName = "sudb", UserName = "suid", Password = "supw"}
                    };
                    
      using(var conn = manager.CreateContentConnection())
      {
        var result = new DbConnectionStringBuilder {ConnectionString = conn.ConnectionString};

        Assert.AreEqual(manager.Description.ConnectionInfo.DatabaseName, result[PgDbConnectionInfo.DatabaseNameKey]);
        Assert.AreEqual("suid", result[PgDbConnectionInfo.UserNameKey]);
        Assert.AreEqual("supw", result[PgDbConnectionInfo.PasswordKey]);
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
      var result = Assert.Throws<ArgumentException>(() => action.Invoke(manager));
      Assert.AreEqual("Description", result.ParamName);
      Console.WriteLine(result.Message);

      manager = new PgDbManager { Superuser = null };
      result = Assert.Throws<ArgumentException>(() => action.Invoke(manager));
      Assert.AreEqual("Superuser", result.ParamName);
      Console.WriteLine(result.Message);

      manager = new PgDbManager { Description = new PgDbDescription() };
      result = Assert.Throws<ArgumentException>(() => action.Invoke(manager));
      Assert.AreEqual("Description.ConnectionInfo", result.ParamName);
      Console.WriteLine(result.Message);

      manager = new PgDbManager { Description = new PgDbDescription { ConnectionInfo = new GenericDbConnectionInfo() } };
      result = Assert.Throws<ArgumentException>(() => action.Invoke(manager));
      Assert.AreEqual("Description.ConnectionInfo.ConnectionString", result.ParamName);
      Console.WriteLine(result.Message);

      manager = new PgDbManager { Description = new PgDbDescription { ConnectionInfo = new GenericDbConnectionInfo { ConnectionString = "" } } };
      result = Assert.Throws<ArgumentException>(() => action.Invoke(manager));
      Assert.AreEqual("Description.ConnectionInfo.ConnectionString", result.ParamName);
      Console.WriteLine(result.Message);
    }

    [Test]
    public void CreateFromDescriptionWithNoConnectionThrows()
    {
      var manager = new PgDbManager {Description = new PgDbDescription {XmlRoot = Resources.TestDescriptionNoConnectionName}};

      var result = Assert.Throws<ArgumentException>(manager.Create);
      Assert.AreEqual("Description.ConnectionInfo", result.ParamName);
      Console.WriteLine(result.Message);
    }

    [Test]
    public void SeedFromDescriptionWithNoConnectionThrows()
    {
      var manager = new PgDbManager {Description = new PgDbDescription {XmlRoot = Resources.TestDescriptionNoConnectionName}};

      var result = Assert.Throws<ArgumentException>(manager.Seed);
      Assert.AreEqual("Description.ConnectionInfo", result.ParamName);
      Console.WriteLine(result.Message);
    }

    [Test]
    public void DestroyFromDescriptionWithNoConnectionThrows()
    {
      var manager = new PgDbManager {Description = new PgDbDescription {XmlRoot = Resources.TestDescriptionNoConnectionName}};

      var result = Assert.Throws<ArgumentException>(manager.Destroy);
      Assert.AreEqual("Description.ConnectionInfo", result.ParamName);
      Console.WriteLine(result.Message);
    }
  }
}
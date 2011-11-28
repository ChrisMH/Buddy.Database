using System;
using System.Data.Common;
using System.Xml.Linq;
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
      var manager = new PgDbManager(new PgDbDescription {ConnectionInfo = GlobalTest.ConnectionInfo1});

      Assert.NotNull(manager.Superuser);
      Assert.AreEqual("postgres", manager.Superuser.Database);
      Assert.AreEqual("postgres", manager.Superuser.UserId);
      Assert.AreEqual("postgres", manager.Superuser.Password);
    }

    [Test]
    public void PgDbManagerCreatedWithSuperuserUsesSuperuser()
    {
      var manager = new PgDbManager(new PgDbDescription {ConnectionInfo = GlobalTest.ConnectionInfo1},
                                    new PgSuperuser {Database = "sudb", UserId = "suid", Password = "supw"});

      Assert.NotNull(manager.Superuser);
      Assert.AreEqual("sudb", manager.Superuser.Database);
      Assert.AreEqual("suid", manager.Superuser.UserId);
      Assert.AreEqual("supw", manager.Superuser.Password);
    }

    [Test]
    public void CreateDatabaseConnectionStringUsesSuperuserParameters()
    {
      var createDatabaseConnectionString = new DbConnectionStringBuilder
                                           {
                                             ConnectionString = PgDbManager.CreateDatabaseConnectionString(GlobalTest.ConnectionInfo1,
                                                                                                           new PgSuperuser {Database = "sudb", UserId = "suid", Password = "supw"})
                                           };
      Assert.AreEqual("sudb", createDatabaseConnectionString[PgDbManager.DatabaseKey]);
      Assert.AreEqual("suid", createDatabaseConnectionString[PgDbManager.UserIdKey]);
      Assert.AreEqual("supw", createDatabaseConnectionString[PgDbManager.PasswordKey]);
    }

    [Test]
    public void CreateContentConnectionStringUsesSuperuserParameters()
    {
      var connectionString = new DbConnectionStringBuilder {ConnectionString = GlobalTest.ConnectionInfo1.ConnectionString};
      var createDatabaseConnectionString = new DbConnectionStringBuilder
                                           {
                                             ConnectionString = PgDbManager.CreateContentConnectionString(GlobalTest.ConnectionInfo1,
                                                                                                          new PgSuperuser {Database = "sudb", UserId = "suid", Password = "supw"})
                                           };
      Assert.AreEqual(connectionString[PgDbManager.DatabaseKey], createDatabaseConnectionString[PgDbManager.DatabaseKey]);
      Assert.AreEqual("suid", createDatabaseConnectionString[PgDbManager.UserIdKey]);
      Assert.AreEqual("supw", createDatabaseConnectionString[PgDbManager.PasswordKey]);
    }


    [Test]
    public void CreateDatabaseConnectionStringThrowsWhenParametersAreInvalid()
    {
      Assert.AreEqual("connectionInfo",
                      Assert.Throws<ArgumentNullException>(() => PgDbManager.CreateDatabaseConnectionString(null, new PgSuperuser())).ParamName);
      Assert.AreEqual("superuser",
                      Assert.Throws<ArgumentNullException>(() => PgDbManager.CreateDatabaseConnectionString(new DbConnectionInfo(), null)).ParamName);
      Assert.AreEqual("connectionInfo.ConnectionString",
                      Assert.Throws<ArgumentException>(() => PgDbManager.CreateDatabaseConnectionString(new DbConnectionInfo {ConnectionString = null}, new PgSuperuser())).ParamName);
      Assert.AreEqual("connectionInfo.ConnectionString",
                      Assert.Throws<ArgumentException>(() => PgDbManager.CreateDatabaseConnectionString(new DbConnectionInfo {ConnectionString = ""}, new PgSuperuser())).ParamName);
    }

    [Test]
    public void CreateContentConnectionStringThrowsWhenParametersAreInvalid()
    {
      Assert.AreEqual("connectionInfo",
                      Assert.Throws<ArgumentNullException>(() => PgDbManager.CreateContentConnectionString(null, new PgSuperuser())).ParamName);
      Assert.AreEqual("superuser",
                      Assert.Throws<ArgumentNullException>(() => PgDbManager.CreateContentConnectionString(new DbConnectionInfo(), null)).ParamName);
      Assert.AreEqual("connectionInfo.ConnectionString",
                      Assert.Throws<ArgumentException>(() => PgDbManager.CreateContentConnectionString(new DbConnectionInfo {ConnectionString = null}, new PgSuperuser())).ParamName);
      Assert.AreEqual("connectionInfo.ConnectionString",
                      Assert.Throws<ArgumentException>(() => PgDbManager.CreateContentConnectionString(new DbConnectionInfo {ConnectionString = ""}, new PgSuperuser())).ParamName);
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
      var manager = new PgDbManager(new PgDbDescription());
      Assert.AreEqual("Description.ConnectionInfo",
                      Assert.Throws<ArgumentNullException>(() => action.Invoke(manager)).ParamName);

      manager = new PgDbManager(new PgDbDescription {ConnectionInfo = new DbConnectionInfo()});
      Assert.AreEqual("Description.ConnectionInfo.ConnectionString",
                      Assert.Throws<ArgumentException>(() => action.Invoke(manager)).ParamName);

      manager = new PgDbManager(new PgDbDescription {ConnectionInfo = new DbConnectionInfo {ConnectionString = ""}});
      Assert.AreEqual("Description.ConnectionInfo.ConnectionString",
                      Assert.Throws<ArgumentException>(() => action.Invoke(manager)).ParamName);

      manager = new PgDbManager(new PgDbDescription {ConnectionInfo = new DbConnectionInfo {ConnectionString = "database=database"}});
      Assert.AreEqual("Description.ConnectionInfo.ProviderFactory",
                      Assert.Throws<ArgumentException>(() => action.Invoke(manager)).ParamName);
    }

    [Test]
    public void CreateFromDescriptionWithNoConnectionThrows()
    {
      var manager = new PgDbManager(new PgDbDescription(XElement.Parse(Resources.TestDescriptionNoConnectionName)));

      Assert.AreEqual("Description.ConnectionInfo", Assert.Throws<ArgumentNullException>(manager.Create).ParamName);
    }
    
    [Test]
    public void SeedFromDescriptionWithNoConnectionThrows()
    {
      var manager = new PgDbManager(new PgDbDescription(XElement.Parse(Resources.TestDescriptionNoConnectionName)));

      Assert.AreEqual("Description.ConnectionInfo", Assert.Throws<ArgumentNullException>(manager.Seed).ParamName);
    }

    [Test]
    public void DestroyFromDescriptionWithNoConnectionThrows()
    {
      var manager = new PgDbManager(new PgDbDescription(XElement.Parse(Resources.TestDescriptionNoConnectionName)));

      Assert.AreEqual("Description.ConnectionInfo", Assert.Throws<ArgumentNullException>(manager.Destroy).ParamName);
    }
  }
}
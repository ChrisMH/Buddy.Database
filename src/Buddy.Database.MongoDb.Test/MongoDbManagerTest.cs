using System;
using NUnit.Framework;

namespace Utility.Database.MongoDb.Test
{
  public class MongoDbManagerTest
  {
    [SetUp]
    public void SetUp()
    {
    }

    [TearDown]
    public void TearDown()
    {
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

    private void InvalidParameterTests(Action<MongoDbManager> action)
    {
      var manager = new MongoDbManager();
      Assert.That(() => action(manager), Throws.ArgumentException.With.Property("ParamName").EqualTo("Description"));

      manager = new MongoDbManager {Description = new DbDescription()};
      Assert.That(() => action(manager), Throws.ArgumentException.With.Property("ParamName").EqualTo("Description.ConnectionInfo"));

      manager = new MongoDbManager {Description = new DbDescription {ConnectionInfo = new DbConnectionInfo()}};
      Assert.That(() => action(manager), Throws.ArgumentException.With.Property("ParamName").EqualTo("Description.ConnectionInfo.ConnectionString"));
    }
  }
}
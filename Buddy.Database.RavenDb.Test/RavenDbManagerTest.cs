using System;
using NUnit.Framework;

namespace Utility.Database.RavenDb.Test
{
  public class RavenDbManagerTest
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

    private void InvalidParameterTests(Action<RavenDbManager> action)
    {
      var manager = new RavenDbManager();
      Assert.That(() => action(manager), Throws.ArgumentException.With.Property("ParamName").EqualTo("Description"));

      manager = new RavenDbManager {Description = new DbDescription()};
      Assert.That(() => action(manager), Throws.ArgumentException.With.Property("ParamName").EqualTo("Description.ConnectionInfo"));

      manager = new RavenDbManager {Description = new DbDescription {ConnectionInfo = new DbConnectionInfo()}};
      Assert.That(() => action(manager), Throws.ArgumentException.With.Property("ParamName").EqualTo("Description.ConnectionInfo.ConnectionString"));
    }
  }
}
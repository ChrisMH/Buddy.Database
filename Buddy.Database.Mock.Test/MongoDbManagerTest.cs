using System;
using NUnit.Framework;

namespace Utility.Database.Mock.Test
{
  public class MockDbManagerTest
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

    private void InvalidParameterTests(Action<MockDbManager> action)
    {
      var manager = new MockDbManager();
      Assert.That(() => action(manager), Throws.ArgumentException.With.Property("ParamName").EqualTo("Description"));

      manager = new MockDbManager { Description = new DbDescription() };
      Assert.That(() => action(manager), Throws.ArgumentException.With.Property("ParamName").EqualTo("Description.ConnectionInfo"));
    }
  }
}
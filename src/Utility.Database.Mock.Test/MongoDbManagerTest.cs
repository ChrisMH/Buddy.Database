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
      Assert.AreEqual("Description",
                      Assert.Throws<ArgumentNullException>(() => action.Invoke(manager)).ParamName);

      manager = new MockDbManager { Description = new MockDbDescription() };
      Assert.AreEqual("Description.ConnectionInfo",
                      Assert.Throws<ArgumentNullException>(() => action.Invoke(manager)).ParamName);
    }
  }
}
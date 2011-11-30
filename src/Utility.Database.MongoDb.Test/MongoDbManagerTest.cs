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
      Assert.AreEqual("Description",
                      Assert.Throws<ArgumentNullException>(() => action.Invoke(manager)).ParamName);

      manager = new MongoDbManager { Description = new MongoDbDescription() };
      Assert.AreEqual("Description.ConnectionInfo",
                      Assert.Throws<ArgumentNullException>(() => action.Invoke(manager)).ParamName);
    }
  }
}
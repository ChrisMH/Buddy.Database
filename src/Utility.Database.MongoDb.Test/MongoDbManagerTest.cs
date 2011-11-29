using System;
using NUnit.Framework;
using Utility.Database.Management.MongoDb;

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
    public void MongoDbManagerCreatedWithNullDescriptionThrows()
    {
      var result = Assert.Throws<ArgumentNullException>(() => new MongoDbManager(null));
      Assert.AreEqual("description", result.ParamName);
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
      var manager = new MongoDbManager(new GenericDbDescription());
      Assert.AreEqual("connectionInfo",
                      Assert.Throws<ArgumentNullException>(() => action.Invoke(manager)).ParamName);

      manager = new MongoDbManager(new GenericDbDescription {ConnectionInfo = new GenericDbConnectionInfo()});
      Assert.AreEqual("connectionInfo.ConnectionString",
                      Assert.Throws<ArgumentException>(() => action.Invoke(manager)).ParamName);

      manager = new MongoDbManager(new GenericDbDescription {ConnectionInfo = new GenericDbConnectionInfo {ConnectionString = ""}});
      Assert.AreEqual("connectionInfo.ConnectionString",
                      Assert.Throws<ArgumentException>(() => action.Invoke(manager)).ParamName);
                      
      manager = new MongoDbManager(new GenericDbDescription {ConnectionInfo = new GenericDbConnectionInfo {ConnectionString = "invalid"}});
      Assert.AreEqual("connectionInfo.ConnectionString",
                      Assert.Throws<ArgumentException>(() => action.Invoke(manager)).ParamName);

      manager = new MongoDbManager(new GenericDbDescription {ConnectionInfo = new GenericDbConnectionInfo {ConnectionString = "mongodb://localhost"}});
      Assert.AreEqual("connectionInfo.ConnectionString",
                      Assert.Throws<ArgumentException>(() => action.Invoke(manager)).ParamName);
      
    }
  }
}
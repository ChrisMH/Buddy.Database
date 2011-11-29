using NUnit.Framework;
using Utility.Database.Management.MongoDb;

namespace Utility.Database.MongoDb.Test
{
  public class MongoDbManagerDestroyTest
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
    public void DestroyDestroysDatabase()
    {
      var manager = new MongoDbManager(new GenericDbDescription {ConnectionInfo = GlobalTest.ConnectionInfo1});

      manager.Create();
      manager.Destroy();
      
      var connectionParams = MongoDbManager.ParseConnectionString(GlobalTest.ConnectionInfo1);
      var server = MongoDB.Driver.MongoServer.Create(connectionParams.ConnectionString);
      Assert.IsFalse(server.DatabaseExists(connectionParams.DatabaseName));
    }
  }
}
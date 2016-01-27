using NUnit.Framework;

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
      var manager = new MongoDbManager { Description = new DbDescription { ConnectionInfo = GlobalTest.DbManager1.Description.ConnectionInfo } };

      manager.Create();
      manager.Destroy();
      
      var server = manager.CreateServer();
      Assert.IsFalse(server.DatabaseExists(manager.Description.ConnectionInfo.DatabaseName));
    }
  }
}
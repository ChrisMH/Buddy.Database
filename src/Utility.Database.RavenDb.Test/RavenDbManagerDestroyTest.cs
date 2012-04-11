using NUnit.Framework;

namespace Utility.Database.RavenDb.Test
{
  public class RavenDbManagerDestroyTest
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
      var manager = new RavenDbManager { Description = new DbDescription { ConnectionInfo = GlobalTest.DbManager1.Description.ConnectionInfo } };

      manager.Create();
      manager.Destroy();
      
      var server = manager.CreateServer();
      Assert.IsFalse(server.DatabaseExists(manager.Description.ConnectionInfo.DatabaseName));
    }
  }
}
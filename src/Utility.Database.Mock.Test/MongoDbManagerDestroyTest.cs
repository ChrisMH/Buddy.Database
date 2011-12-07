using System;
using NUnit.Framework;

namespace Utility.Database.Mock.Test
{
  public class MockDbManagerDestroyTest
  {
    [SetUp]
    public void SetUp()
    {
      MockDatabaseProvider.Reset();
    }

    [TearDown]
    public void TearDown()
    {
      MockDatabaseProvider.Reset();
    }

    [Test]
    public void DestroyDestroysDatabase()
    {
      var manager = new MockDbManager
      {
        Description = new MockDbDescription
        {
          XmlRoot = DbDescriptions.Valid,
          ConnectionInfo = GlobalTest.DbManager1.Description.ConnectionInfo
        }
      };

      manager.Create();
      manager.Destroy();

      Assert.Throws<ArgumentException>(() => MockDatabaseProvider.Open(GlobalTest.DbManager1.Description.ConnectionInfo));
    }
  }
}
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Utility.Database.Mock.Test
{
  public class MockDbManagerSeedTest
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
    public void SeedSeedsDatabase()
    {
      var manager = new MockDbManager
      {
        Description = new DbDescription
        {
          XmlRoot = DbDescriptions.Valid,
          ConnectionInfo = GlobalTest.DbManager1.Description.ConnectionInfo
        }
      };

      manager.Create();
      manager.Seed();

      var db = MockDatabaseProvider.Open(GlobalTest.DbManager1.Description.ConnectionInfo) as TestMockDatabase;

      var row = db.Table.SingleOrDefault(r => r.Id == 3);
      Assert.That(row, Is.Not.Null);
    }

    [Test]
    public void SeedWithInvalidSeedTypeThrows()
    {
      var manager = new MockDbManager
      {
        Description = new DbDescription
        {
          XmlRoot = DbDescriptions.InvalidSeedType,
          ConnectionInfo = GlobalTest.DbManager1.Description.ConnectionInfo
        }
      };

      manager.Create();

      Assert.That(() => manager.Seed(), Throws.ArgumentException.With.Property("ParamName").EqualTo("ScriptType"));
    }
  }
}
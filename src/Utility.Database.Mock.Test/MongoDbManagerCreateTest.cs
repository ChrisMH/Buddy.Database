using NUnit.Framework;

namespace Utility.Database.Mock.Test
{
  public class MockDbManagerCreateTest
  {
    public const string LiteralSchema = "db.createCollection('c1');db.createCollection('c2');";

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
    public void CreateCreatesDatabase()
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

      Assert.That(MockDatabaseProvider.Open(GlobalTest.DbManager1.Description.ConnectionInfo), Is.Not.Null);
    }

    [Test]
    public void CreateWithInvalidSchemaTypeThrows()
    {
      var manager = new MockDbManager
                      {
                        Description = new DbDescription
                                        {
                                          XmlRoot = DbDescriptions.InvalidSchemaType,
                                          ConnectionInfo = GlobalTest.DbManager1.Description.ConnectionInfo
                                        }
                      };

      Assert.That(() => manager.Create(), Throws.ArgumentException.With.Property("ParamName").EqualTo("ScriptType"));
    }
  }
}
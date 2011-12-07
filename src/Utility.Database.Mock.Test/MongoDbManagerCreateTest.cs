using System;
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
                        Description = new MockDbDescription
                                        {
                                          XmlRoot = DbDescriptions.Valid,
                                          ConnectionInfo = GlobalTest.DbManager1.Description.ConnectionInfo
                                        }
                      };

      manager.Create();

      Assert.IsNotNull(MockDatabaseProvider.Open(GlobalTest.DbManager1.Description.ConnectionInfo));
    }

    [Test]
    public void CreateWithInvalidSchemaTypeThrows()
    {
      var manager = new MockDbManager
                      {
                        Description = new MockDbDescription
                                        {
                                          XmlRoot = DbDescriptions.InvalidSchemaType,
                                          ConnectionInfo = GlobalTest.DbManager1.Description.ConnectionInfo
                                        }
                      };
      var result = Assert.Throws<ArgumentException>(manager.Create);
      Assert.AreEqual("ScriptType", result.ParamName);
      Console.WriteLine(result.Message);
    }
  }
}
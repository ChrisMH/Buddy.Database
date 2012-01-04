using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Utility.Database.MongoDb.Test
{
  public class MongoDbManagerCreateTest
  {
    public const string LiteralSchema = "db.createCollection('c1');db.createCollection('c2');";

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
    public void CreateCreatesDatabase()
    {
      var manager = new MongoDbManager {Description = new DbDescription {ConnectionInfo = GlobalTest.DbManager1.Description.ConnectionInfo}};

      manager.Create();

      var server = manager.CreateServer();
      Assert.IsTrue(server.DatabaseExists(manager.Description.ConnectionInfo.DatabaseName));
    }

    [Test]
    public void CreateFromDescriptionCreatesDatabase()
    {
      var manager = DbManager.Create(Resources.description);
      manager.Create();

      var server = GlobalTest.DbManager1.CreateServer();
      Assert.IsTrue(server.DatabaseExists(manager.Description.ConnectionInfo.DatabaseName));
    }


    [Test]
    public void CreateWithLiteralSchemaCreatesSchema()
    {
      var manager = new MongoDbManager
                      {
                        Description = new DbDescription
                                        {
                                          ConnectionInfo = GlobalTest.DbManager1.Description.ConnectionInfo,
                                          Schemas = new List<DbScript> {new DbScript {ScriptType = ScriptType.Literal, ScriptValue = LiteralSchema}}
                                        }
                      };

      manager.Create();

      var db = manager.CreateDatabase();

      var result = db.GetCollectionNames().FirstOrDefault(c => c == "c1");
      Assert.IsNotNull(result);
    }
  }
}
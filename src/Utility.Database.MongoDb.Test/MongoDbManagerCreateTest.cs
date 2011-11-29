using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using NUnit.Framework;
using Utility.Database.Management.MongoDb;

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
      var manager = new MongoDbManager(new GenericDbDescription {ConnectionInfo = GlobalTest.ConnectionInfo1});

      manager.Create();

      var connectionParams = MongoDbManager.ParseConnectionString(GlobalTest.ConnectionInfo1);
      var server = MongoServer.Create(connectionParams.ConnectionString);
      Assert.IsTrue(server.DatabaseExists(connectionParams.DatabaseName));
    }

    [Test]
    public void CreateWithLiteralSchemaCreatesSchema()
    {
      var manager = new MongoDbManager(new GenericDbDescription
                                       {
                                         ConnectionInfo = GlobalTest.ConnectionInfo1,
                                         Schemas = new List<DbScript> {new DbScript {ScriptType = ScriptType.Literal, ScriptValue = LiteralSchema}}
                                       });

      manager.Create();
      
      var connectionParams = MongoDbManager.ParseConnectionString(GlobalTest.ConnectionInfo1);
      var server = MongoServer.Create(connectionParams.ConnectionString);
      var db = server.GetDatabase(connectionParams.DatabaseName);

      var result = ((MongoDatabase) db).GetCollectionNames().FirstOrDefault(c => c == "c1");
      Assert.IsNotNull(result);

    }
  }
}
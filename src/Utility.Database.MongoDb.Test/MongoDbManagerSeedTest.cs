using System.Collections.Generic;
using NUnit.Framework;

namespace Utility.Database.MongoDb.Test
{
  public class MongoDbManagerSeedTest
  {
    public const string LiteralSchema = "db.createCollection('c1');db.createCollection('c2');";
    public const string LiteralSeed = "db.c1.insert({name: 'Fred'});db.c1.insert({name: 'Jim', age: 39});db.c2.insert({username: 'fred', identity: 'abaadd'});";

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
    public void SeedFromLiteralSeedsDatabase()
    {
      var manager = new MongoDbManager
                    {
                      Description = new MongoDbDescription
                                    {
                                      ConnectionInfo = GlobalTest.DbManager1.ConnectionInfo,
                                      Schemas = new List<DbScript> {new DbScript {ScriptType = ScriptType.Literal, ScriptValue = LiteralSchema}},
                                      Seeds = new List<DbScript> {new DbScript {ScriptType = ScriptType.Literal, ScriptValue = LiteralSeed}}
                                    }
                    };

      manager.Create();
      manager.Seed();

      var db = manager.CreateDatabase();

      Assert.That(db["c1"].Count() == 2);
      Assert.That(db["c2"].Count() == 1);
    }

    [Test]
    public void SeedFromDescriptionSeedsDatabase()
    {
      var manager = new MongoDbManager
                    {
                      Description = new MongoDbDescription
                                    {
                                      XmlRoot = Resources.description
                                    }
                    };
      manager.Create();
      manager.Seed();
    }
  }
}
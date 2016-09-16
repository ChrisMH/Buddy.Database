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
                      Description = new DbDescription
                                    {
                                      ConnectionInfo = GlobalTest.DbManager1.Description.ConnectionInfo,
                                      Schemas = new List<DbScript> {new DbScript {ScriptType = ScriptType.Literal, ScriptValue = LiteralSchema}},
                                      Seeds = new List<DbScript> {new DbScript {ScriptType = ScriptType.Literal, ScriptValue = LiteralSeed}}
                                    }
                    };

      manager.Create();
      manager.Seed();

      var db = manager.CreateDatabase();

      Assert.That(db["c1"].Count(), Is.EqualTo(2));
      Assert.That(db["c2"].Count(), Is.EqualTo(1));
    }

    [Test]
    public void SeedFromDescriptionSeedsDatabase()
    {
      var manager = DbManager.Create(Resources.description);
      manager.Create();
      manager.Seed();

      var db = GlobalTest.DbManager1.CreateDatabase();

      Assert.That(db["role"].Count(), Is.EqualTo(5));
      Assert.That(db["user"].Count(), Is.EqualTo(2));
    }
  }
}
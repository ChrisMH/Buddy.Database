using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using NUnit.Framework;
using Utility.Database.Management.MongoDb;

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
          var manager = new MongoDbManager(new GenericDbDescription
                                       {
                                         ConnectionInfo = GlobalTest.ConnectionInfo1,
                                         Schemas = new List<DbScript> {new DbScript {ScriptType = ScriptType.Literal, ScriptValue = LiteralSchema}},
                                         Seeds = new List<DbScript> { new DbScript {ScriptType = ScriptType.Literal, ScriptValue = LiteralSeed}}
                                       });

      manager.Create();
      manager.Seed();

      var connectionParams = MongoDbManager.ParseConnectionString(GlobalTest.ConnectionInfo1);
      var server = MongoServer.Create(connectionParams.ConnectionString);
      var db = server.GetDatabase(connectionParams.DatabaseName);

      Assert.That(((MongoCollection) db["c1"]).Count() == 2);
      Assert.That(((MongoCollection) db["c2"]).Count() == 1);
    }
  }
}
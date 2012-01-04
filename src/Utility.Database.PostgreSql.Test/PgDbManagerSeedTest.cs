using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Utility.Database.PostgreSql.Test
{
  public class PgDbManagerSeedTest
  {
    private const string TestSchema = "CREATE SCHEMA test_schema; CREATE TABLE test_schema.test_table ( id serial NOT NULL, name varchar NOT NULL );";
    private const string TestSeed = "INSERT INTO test_schema.test_table (name) VALUES('name1');INSERT INTO test_schema.test_table (name) VALUES('name2');";

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
    public void SeedSeedsDatabase()
    {
      var manager = new PgDbManager
                    {
                      Description = new PgDbDescription
                                    {
                                      ConnectionInfo = GlobalTest.Manager1.Description.ConnectionInfo,
                                      Schemas = new List<DbScript> {new DbScript {ScriptType = ScriptType.Literal, ScriptValue = TestSchema}},
                                      Seeds = new List<DbScript> {new DbScript {ScriptType = ScriptType.Literal, ScriptValue = TestSeed}}
                                    }
                    };

      manager.Create();

      manager.Seed();

      using (var conn = manager.CreateContentConnection())
      {
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandText = "SELECT COUNT(*) FROM test_schema.test_table";
          Assert.AreEqual(2, Convert.ToInt64(cmd.ExecuteScalar()));
        }
      }
    }

    [Test]
    public void SeedFromDescriptionSeedsDatabase()
    {
      var manager = DbManager.Create(Resources.TestDescription);

      manager.Create();
      manager.Seed();

      using (var conn = GlobalTest.Manager1.CreateContentConnection())
      {
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandText = "SELECT COUNT(*) FROM test_schema.test_table";
          Assert.AreEqual(3, Convert.ToInt64(cmd.ExecuteScalar()));
        }
      }
    }
  }
}
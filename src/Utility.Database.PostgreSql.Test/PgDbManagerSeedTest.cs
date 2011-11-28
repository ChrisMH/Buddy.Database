using System;
using System.Collections.Generic;
using System.Xml.Linq;
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
      var manager = new PgDbManager(
        new PgDbDescription
        {
          ConnectionInfo = GlobalTest.ConnectionInfo1,
          Schemas = new List<DbScript> {new DbScript {ScriptType = ScriptType.Literal, ScriptValue = TestSchema}},
          Seeds = new List<DbScript> {new DbScript {ScriptType = ScriptType.Literal, ScriptValue = TestSeed}}
        });

      manager.Create();

      manager.Seed();

      using (var conn = GlobalTest.ConnectionInfo1.ProviderFactory.CreateConnection())
      {
        conn.ConnectionString = PgDbManager.CreateContentConnectionString(GlobalTest.ConnectionInfo1, GlobalTest.Superuser);
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
      var manager = new PgDbManager(new PgDbDescription(XElement.Parse(Resources.TestDescription)));

      manager.Create();
      manager.Seed();

      using (var conn = GlobalTest.ConnectionInfo1.ProviderFactory.CreateConnection())
      {
        conn.ConnectionString = PgDbManager.CreateContentConnectionString(GlobalTest.ConnectionInfo1, GlobalTest.Superuser);
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
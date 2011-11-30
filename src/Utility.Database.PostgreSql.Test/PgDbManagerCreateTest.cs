using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Utility.Database.PostgreSql.Test
{
  public class PgDbManagerCreateTest
  {
    private const string TestSchema = "CREATE SCHEMA test_schema; CREATE TABLE test_schema.test_table ( id serial NOT NULL, name varchar NOT NULL );";

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
      var manager = new PgDbManager {Description = new PgDbDescription {ConnectionInfo = GlobalTest.ConnectionInfo1}};

      manager.Create();

      using (var conn = manager.CreateDatabaseConnection())
      {
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandText = string.Format("SELECT COUNT(*) FROM pg_catalog.pg_database WHERE datname='{0}'", manager.ConnectionInfo[PgDbManager.DatabaseKey]);
          Assert.AreEqual(1, Convert.ToInt64(cmd.ExecuteScalar()));
        }
      }
    }


    [Test]
    public void CreateWithTemplateCreatesDatabaseWithTemplate()
    {
      var manager = new PgDbManager
                    {
                      Description = new PgDbDescription
                                    {
                                      ConnectionInfo = GlobalTest.ConnectionInfo1,
                                      TemplateName = "template_postgis"
                                    }
                    };

      manager.Create();

      using (var conn = manager.CreateContentConnection())
      {
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandText = "SELECT COUNT(*) FROM pg_catalog.pg_tables WHERE schemaname='public' AND tablename='geometry_columns'";
          Assert.AreEqual(1, Convert.ToInt64(cmd.ExecuteScalar()));
        }
      }
    }


    [Test]
    public void CreateCreatesRole()
    {
      var manager = new PgDbManager {Description = new PgDbDescription {ConnectionInfo = GlobalTest.ConnectionInfo1}};


      manager.Create();

      using (var conn = manager.CreateContentConnection())
      {
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandText = string.Format("SELECT COUNT(*) FROM pg_catalog.pg_user WHERE usename='{0}'", manager.ConnectionInfo[PgDbManager.UserIdKey]);
          Assert.AreEqual(1, Convert.ToInt64(cmd.ExecuteScalar()));
        }
      }
    }

    [Test]
    public void CreateWithSchemaCreatesSchema()
    {
      var manager = new PgDbManager
                    {
                      Description =
                        new PgDbDescription
                        {
                          ConnectionInfo = GlobalTest.ConnectionInfo1,
                          Schemas = new List<DbScript> {new DbScript {ScriptType = ScriptType.Literal, ScriptValue = TestSchema}}
                        }
                    };

      manager.Create();

      using (var conn = manager.CreateContentConnection())
      {
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandText = "SELECT COUNT(*) FROM pg_catalog.pg_namespace WHERE nspname='test_schema'";
          Assert.AreEqual(1, Convert.ToInt64(cmd.ExecuteScalar()));
        }
      }
    }


    [Test]
    public void CreateGrantsPermissionsOnPublicSchema()
    {
      var manager = new PgDbManager
                    {
                      Description =
                        new PgDbDescription
                        {
                          ConnectionInfo = GlobalTest.ConnectionInfo1,
                          Schemas = new List<DbScript> {new DbScript {ScriptType = ScriptType.Literal, ScriptValue = TestSchema}}
                        }
                    };

      manager.Create();

      using (var conn = manager.CreateContentConnection())
      {
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandText = "SELECT nspacl FROM pg_catalog.pg_namespace WHERE nspname='public'";
          var result = Convert.ToString(cmd.ExecuteScalar());
          Assert.That(result.Contains(string.Format("{0}=UC", manager.ConnectionInfo[PgDbManager.UserIdKey])));
        }
      }
    }

    [Test]
    public void CreateWithSchemaGrantsPermissionsOnSchema()
    {
      var manager = new PgDbManager
                    {
                      Description =
                        new PgDbDescription
                        {
                          ConnectionInfo = GlobalTest.ConnectionInfo1,
                          Schemas = new List<DbScript> {new DbScript {ScriptType = ScriptType.Literal, ScriptValue = TestSchema}}
                        }
                    };

      manager.Create();

      using (var conn = manager.CreateContentConnection())
      {
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandText = "SELECT nspacl FROM pg_catalog.pg_namespace WHERE nspname='test_schema'";
          var result = Convert.ToString(cmd.ExecuteScalar());
          Assert.That(result.Contains(string.Format("{0}=UC", manager.ConnectionInfo[PgDbManager.UserIdKey])));
        }
      }
    }

    [Test]
    public void CreateFromDescriptionWithTemplateCreatesDatabaseWithTemplate()
    {
      var manager = new PgDbManager {Description = new PgDbDescription {XmlRoot = Resources.TestDescriptionWithTemplate}};

      manager.Create();

      using (var conn = manager.CreateContentConnection())
      {
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandText = "SELECT COUNT(*) FROM pg_catalog.pg_tables WHERE schemaname='public' AND tablename='geometry_columns'";
          Assert.AreEqual(1, Convert.ToInt64(cmd.ExecuteScalar()));
        }
      }
    }

    [Test]
    public void CreateFromDescriptionCreatesSchema()
    {
      var manager = new PgDbManager
                    {
                      Description = new PgDbDescription {XmlRoot = Resources.TestDescription}
                    };

      manager.Create();

      using (var conn = manager.CreateContentConnection())
      {
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandText = "SELECT COUNT(*) FROM pg_catalog.pg_namespace WHERE nspname='test_schema'";
          Assert.AreEqual(1, Convert.ToInt64(cmd.ExecuteScalar()));
        }
      }
    }
  }
}
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Xml.Linq;
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
      var manager = new PgDbManager(new PgDbDescription {ConnectionInfo = GlobalTest.ConnectionInfo1});
      var csBuilder = new DbConnectionStringBuilder {ConnectionString = GlobalTest.ConnectionInfo1.ConnectionString};

      manager.Create();

      using (var conn = manager.ConnectionInfo.ProviderFactory.CreateConnection())
      {
        conn.ConnectionString = PgDbManager.CreateDatabaseConnectionString(GlobalTest.ConnectionInfo1, GlobalTest.Superuser);
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandText = string.Format("SELECT COUNT(*) FROM pg_catalog.pg_database WHERE datname='{0}'", csBuilder[PgDbManager.DatabaseKey]);
          Assert.AreEqual(1, Convert.ToInt64(cmd.ExecuteScalar()));
        }
      }
    }


    [Test]
    public void CreateWithTemplateCreatesDatabaseWithTemplate()
    {
      var manager = new PgDbManager(new PgDbDescription
                                    {
                                      ConnectionInfo = GlobalTest.ConnectionInfo1,
                                      TemplateName = "template_postgis"
                                    });

      manager.Create();

      using (var conn = manager.ConnectionInfo.ProviderFactory.CreateConnection())
      {
        conn.ConnectionString = PgDbManager.CreateContentConnectionString(GlobalTest.ConnectionInfo1, GlobalTest.Superuser);
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
      var manager = new PgDbManager(new PgDbDescription {ConnectionInfo = GlobalTest.ConnectionInfo1});
      var csBuilder = new DbConnectionStringBuilder {ConnectionString = GlobalTest.ConnectionInfo1.ConnectionString};


      manager.Create();

      using (var conn = manager.ConnectionInfo.ProviderFactory.CreateConnection())
      {
        conn.ConnectionString = PgDbManager.CreateDatabaseConnectionString(GlobalTest.ConnectionInfo1, GlobalTest.Superuser);
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandText = string.Format("SELECT COUNT(*) FROM pg_catalog.pg_user WHERE usename='{0}'", csBuilder[PgDbManager.UserIdKey]);
          Assert.AreEqual(1, Convert.ToInt64(cmd.ExecuteScalar()));
        }
      }
    }

    [Test]
    public void CreateWithSchemaCreatesSchema()
    {
      var manager = new PgDbManager(
        new PgDbDescription
        {
          ConnectionInfo = GlobalTest.ConnectionInfo1,
          Schemas = new List<DbScript> {new DbScript {ScriptType = ScriptType.Literal, ScriptValue = TestSchema}}
        });
      var csBuilder = new DbConnectionStringBuilder {ConnectionString = GlobalTest.ConnectionInfo1.ConnectionString};

      manager.Create();

      using (var conn = manager.ConnectionInfo.ProviderFactory.CreateConnection())
      {
        conn.ConnectionString = PgDbManager.CreateContentConnectionString(GlobalTest.ConnectionInfo1, GlobalTest.Superuser);
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
      var manager = new PgDbManager(
        new PgDbDescription
        {
          ConnectionInfo = GlobalTest.ConnectionInfo1,
          Schemas = new List<DbScript> {new DbScript {ScriptType = ScriptType.Literal, ScriptValue = TestSchema}}
        });
      var csBuilder = new DbConnectionStringBuilder {ConnectionString = GlobalTest.ConnectionInfo1.ConnectionString};

      manager.Create();

      using (var conn = manager.ConnectionInfo.ProviderFactory.CreateConnection())
      {
        conn.ConnectionString = PgDbManager.CreateContentConnectionString(GlobalTest.ConnectionInfo1, GlobalTest.Superuser);
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandText = "SELECT nspacl FROM pg_catalog.pg_namespace WHERE nspname='public'";
          var result = Convert.ToString(cmd.ExecuteScalar());
          Assert.That(result.Contains(string.Format("{0}=UC", csBuilder[PgDbManager.UserIdKey])));
        }
      }
    }

    [Test]
    public void CreateWithSchemaGrantsPermissionsOnSchema()
    {
      var manager = new PgDbManager(
        new PgDbDescription
        {
          ConnectionInfo = GlobalTest.ConnectionInfo1,
          Schemas = new List<DbScript> {new DbScript {ScriptType = ScriptType.Literal, ScriptValue = TestSchema}}
        });
      var csBuilder = new DbConnectionStringBuilder {ConnectionString = GlobalTest.ConnectionInfo1.ConnectionString};

      manager.Create();

      using (var conn = manager.ConnectionInfo.ProviderFactory.CreateConnection())
      {
        conn.ConnectionString = PgDbManager.CreateContentConnectionString(GlobalTest.ConnectionInfo1, GlobalTest.Superuser);
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandText = "SELECT nspacl FROM pg_catalog.pg_namespace WHERE nspname='test_schema'";
          var result = Convert.ToString(cmd.ExecuteScalar());
          Assert.That(result.Contains(string.Format("{0}=UC", csBuilder[PgDbManager.UserIdKey])));
        }
      }
    }

    [Test]
    public void CreateFromDescriptionWithTemplateCreatesDatabaseWithTemplate()
    {
      var manager = new PgDbManager(new PgDbDescription(XElement.Parse(Resources.TestDescriptionWithTemplate)));

      manager.Create();

      using (var conn = manager.ConnectionInfo.ProviderFactory.CreateConnection())
      {
        conn.ConnectionString = PgDbManager.CreateContentConnectionString(GlobalTest.ConnectionInfo1, GlobalTest.Superuser);
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
      var manager = new PgDbManager(new PgDbDescription(XElement.Parse(Resources.TestDescription)));

      manager.Create();

      using (var conn = GlobalTest.ConnectionInfo1.ProviderFactory.CreateConnection())
      {
        conn.ConnectionString = PgDbManager.CreateContentConnectionString(GlobalTest.ConnectionInfo1, GlobalTest.Superuser);
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
using System.Collections.Generic;
using NUnit.Framework;

namespace Utility.Database.PostgreSql.Test
{
  public class PgCreatorTest
  {
    [Test]
    public void SchemaConnectionStringAttributeIsRemoved()
    {
      var creator = new PgCreator(new PgDbDescription {ConnectionName = "ConnectionNameWithSchema"});

      Assert.False(creator.Provider.ConnectionString.ContainsKey("schema"));
    }

    [Test]
    public void DatabaseWithExistingUserNameIsCreated()
    {
      var creator = new PgCreator(new PgDbDescription {ConnectionName = "ConnectionNameWithExistingUserName"});

      try
      {
        creator.Create();

        using (var db = creator.CreateDatabaseProvider.Database)
        {
          Assert.AreEqual(1, db.ExecuteScalar("SELECT COUNT(*) FROM pg_catalog.pg_database WHERE datname=:p0", creator.Provider.ConnectionString["database"]));
        }
      }
      finally
      {
        creator.Destroy();
      }
    }

    [Test]
    public void DatabaseIsCreatedWithTemplate()
    {
      var creator = new PgCreator(new PgDbDescription
                                  {
                                    ConnectionName = "ConnectionNameWithExistingUserName",
                                    TemplateName = "template_postgis"
                                  });

      try
      {
        creator.Create();

        using (var db = creator.Provider.Database)
        {
          Assert.AreEqual(1, db.ExecuteScalar("SELECT COUNT(*) FROM pg_catalog.pg_tables WHERE schemaname=:p0 AND tablename=:p1", "public", "geometry_columns"));
        }
      }
      finally
      {
        creator.Destroy();
      }
    }

    [Test]
    public void DatabaseAndUserWithMissingUserNameAreCreated()
    {
      var creator = new PgCreator(new PgDbDescription {ConnectionName = "ConnectionNameWithMissingUserName"});

      try
      {
        creator.Create();

        using (var db = creator.CreateDatabaseProvider.Database)
        {
          Assert.AreEqual(1, db.ExecuteScalar("SELECT COUNT(*) FROM pg_catalog.pg_database WHERE datname=:p0", creator.Provider.ConnectionString["database"]));
          Assert.AreEqual(1, db.ExecuteScalar("SELECT COUNT(*) FROM pg_catalog.pg_user WHERE usename=:p0", creator.Provider.ConnectionString["user id"]));
        }
      }
      finally
      {
        creator.Destroy();
      }
    }


    [Test]
    public void DatabaseCreationCreatesSchema()
    {
      var creator = new PgCreator(
        new PgDbDescription
        {
          ConnectionName = "ConnectionName",
          Schemas = new List<DbScript> {new DbScript {ScriptType = ScriptType.Literal, ScriptValue = TestSchema}}
        });

      try
      {
        creator.Create();

        using (var db = creator.CreateContentProvider.Database)
        {
          Assert.AreEqual(1, db.ExecuteScalar("SELECT COUNT(*) FROM pg_catalog.pg_namespace WHERE nspname='test_schema'"));
        }
      }
      finally
      {
        creator.Destroy();
      }
    }

    [Test]
    public void DatabaseCreationGrantsPermissionsOnPublicSchema()
    {
      var creator = new PgCreator(
        new PgDbDescription
        {
          ConnectionName = "ConnectionName",
          Schemas = new List<DbScript> {new DbScript {ScriptType = ScriptType.Literal, ScriptValue = TestSchema}}
        });

      try
      {
        creator.Create();

        using (var db = creator.CreateContentProvider.Database)
        {
          var result = db.ExecuteScalar("SELECT nspacl FROM pg_catalog.pg_namespace WHERE nspname='public'");
          Assert.True(result.Contains(string.Format("{0}=UC", creator.Provider.ConnectionString["user id"])));
        }
      }
      finally
      {
        creator.Destroy();
      }
    }

    [Test]
    public void DatabaseCreationGrantsPermissionsOnSchema()
    {
      var creator = new PgCreator(
        new PgDbDescription
        {
          ConnectionName = "ConnectionName",
          Schemas = new List<DbScript> {new DbScript {ScriptType = ScriptType.Literal, ScriptValue = TestSchema}}
        });

      try
      {
        creator.Create();

        using (var db = creator.CreateContentProvider.Database)
        {
          var result = db.ExecuteScalar("SELECT nspacl FROM pg_catalog.pg_namespace WHERE nspname='test_schema'");
          Assert.True(result.Contains(string.Format("{0}=UC", creator.Provider.ConnectionString["user id"])));
        }
      }
      finally
      {
        creator.Destroy();
      }
    }

    [Test]
    public void DatabaseSeedSeedsDatabase()
    {
      var creator = new PgCreator(
        new PgDbDescription
        {
          ConnectionName = "ConnectionName",
          Schemas = new List<DbScript> {new DbScript {ScriptType = ScriptType.Literal, ScriptValue = TestSchema}},
          Seeds = new List<DbScript> {new DbScript {ScriptType = ScriptType.Literal, ScriptValue = TestSeed}}
        });

      try
      {
        creator.Create();

        creator.Seed();

        using (var db = creator.Provider.Database)
        {
          Assert.AreEqual(2, db.ExecuteScalar("SELECT COUNT(*) FROM test_schema.test_table"));
        }
      }
      finally
      {
        creator.Destroy();
      }
    }

    [Test]
    public void NullSuperuserUsesDefaultSuperuser()
    {
      var creator = new PgCreator(new PgDbDescription {ConnectionName = "ConnectionNameWithSchema"});

      Assert.AreEqual("postgres", creator.CreateDatabaseProvider.ConnectionString["database"]);
      Assert.AreEqual("postgres", creator.CreateDatabaseProvider.ConnectionString["user id"]);
      Assert.AreEqual("postgres", creator.CreateDatabaseProvider.ConnectionString["password"]);
      Assert.AreEqual("utility_database_test", creator.CreateContentProvider.ConnectionString["database"]);
      Assert.AreEqual("postgres", creator.CreateContentProvider.ConnectionString["user id"]);
      Assert.AreEqual("postgres", creator.CreateContentProvider.ConnectionString["password"]);
    }

    [Test]
    public void SpecificSuperuserIsUsed()
    {
      var creator = new PgCreator(new PgDbDescription {ConnectionName = "ConnectionNameWithSchema"},
                                  new PgSuperuser {Database = "sudb", UserId = "suid", Password = "supw"});

      Assert.AreEqual("sudb", creator.CreateDatabaseProvider.ConnectionString["database"]);
      Assert.AreEqual("suid", creator.CreateDatabaseProvider.ConnectionString["user id"]);
      Assert.AreEqual("supw", creator.CreateDatabaseProvider.ConnectionString["password"]);
      Assert.AreEqual("utility_database_test", creator.CreateContentProvider.ConnectionString["database"]);
      Assert.AreEqual("suid", creator.CreateContentProvider.ConnectionString["user id"]);
      Assert.AreEqual("supw", creator.CreateContentProvider.ConnectionString["password"]);
    }

    private const string TestSchema = "CREATE SCHEMA test_schema; CREATE TABLE test_schema.test_table ( id serial NOT NULL, name varchar NOT NULL );";
    private const string TestSeed = "INSERT INTO test_schema.test_table (name) VALUES('name1');INSERT INTO test_schema.test_table (name) VALUES('name2');";
  }
}
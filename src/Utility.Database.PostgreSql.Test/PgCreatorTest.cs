using System;
using System.Collections.Generic;
using System.Data.Common;
using NUnit.Framework;
using Utility.Database.Management;
using Utility.Database.Management.PostgreSql;

namespace Utility.Database.PostgreSql.Test
{
  public class PgCreatorTest
  {
    [Test]
    public void DatabaseWithExistingUserNameIsCreated()
    {
      var manager = new PgDbManager(new PgDbDescription {ConnectionInfo = new DbConnectionInfo("ConnectionNameWithExistingUserName")});

      try
      {
        manager.Create();

        using (var conn = manager.ConnectionInfo.ProviderFactory.CreateConnection())
        {
          conn.ConnectionString = manager.CreateContentConnectionString;
          conn.Open();

          using (var cmd = conn.CreateCommand())
          {
            cmd.CommandText = string.Format("SELECT COUNT(*) FROM pg_catalog.pg_database WHERE datname='{0}'", manager.databaseName);
            Assert.AreEqual(1, Convert.ToInt64(cmd.ExecuteScalar()));
          }
        }
      }
      finally
      {
        manager.Destroy();
      }
    }

    [Test]
    public void DatabaseIsCreatedWithTemplate()
    {
      var manager = new PgDbManager(new PgDbDescription
                                    {
                                      ConnectionInfo = new DbConnectionInfo("ConnectionNameWithExistingUserName"),
                                      TemplateName = "template_postgis"
                                    });

      try
      {
        manager.Create();

        using (var conn = manager.ConnectionInfo.ProviderFactory.CreateConnection())
        {
          conn.ConnectionString = manager.CreateContentConnectionString;
          conn.Open();

          using (var cmd = conn.CreateCommand())
          {
            cmd.CommandText = "SELECT COUNT(*) FROM pg_catalog.pg_tables WHERE schemaname='public' AND tablename='geometry_columns'";
            Assert.AreEqual(1, Convert.ToInt64(cmd.ExecuteScalar()));
          }
        }
      }
      finally
      {
        manager.Destroy();
      }
    }

    [Test]
    public void DatabaseAndUserWithMissingUserNameAreCreated()
    {
      var manager = new PgDbManager(new PgDbDescription {ConnectionInfo = new DbConnectionInfo("ConnectionNameWithMissingUserName")});

      try
      {
        manager.Create();

        using (var conn = manager.ConnectionInfo.ProviderFactory.CreateConnection())
        {
          conn.ConnectionString = manager.CreateContentConnectionString;
          conn.Open();

          using (var cmd = conn.CreateCommand())
          {
            cmd.CommandText = string.Format("SELECT COUNT(*) FROM pg_catalog.pg_database WHERE datname='{0}'", manager.databaseName);
            Assert.AreEqual(1, Convert.ToInt64(cmd.ExecuteScalar()));

            cmd.CommandText = string.Format("SELECT COUNT(*) FROM pg_catalog.pg_user WHERE usename='{0}'", manager.userId);
            Assert.AreEqual(1, Convert.ToInt64(cmd.ExecuteScalar()));
          }
        }
      }
      finally
      {
        manager.Destroy();
      }
    }


    [Test]
    public void DatabaseCreationCreatesSchema()
    {
      var manager = new PgDbManager(
        new PgDbDescription
        {
          ConnectionInfo = new DbConnectionInfo("ConnectionName"),
          Schemas = new List<DbScript> {new DbScript {ScriptType = ScriptType.Literal, ScriptValue = TestSchema}}
        });

      try
      {
        manager.Create();

        using (var conn = manager.ConnectionInfo.ProviderFactory.CreateConnection())
        {
          conn.ConnectionString = manager.CreateContentConnectionString;
          conn.Open();

          using (var cmd = conn.CreateCommand())
          {
            cmd.CommandText = "SELECT COUNT(*) FROM pg_catalog.pg_namespace WHERE nspname='test_schema'";
            Assert.AreEqual(1, Convert.ToInt64(cmd.ExecuteScalar()));
          }
        }
      }
      finally
      {
        manager.Destroy();
      }
    }

    [Test]
    public void DatabaseCreationGrantsPermissionsOnPublicSchema()
    {
      var manager = new PgDbManager(
        new PgDbDescription
        {
          ConnectionInfo = new DbConnectionInfo("ConnectionName"),
          Schemas = new List<DbScript> {new DbScript {ScriptType = ScriptType.Literal, ScriptValue = TestSchema}}
        });

      try
      {
        manager.Create();

        using (var conn = manager.ConnectionInfo.ProviderFactory.CreateConnection())
        {
          conn.ConnectionString = manager.CreateContentConnectionString;
          conn.Open();

          using (var cmd = conn.CreateCommand())
          {
            cmd.CommandText = "SELECT nspacl FROM pg_catalog.pg_namespace WHERE nspname='public'";
            var result = Convert.ToString(cmd.ExecuteScalar());
            Assert.That(result.Contains(string.Format("{0}=UC", manager.userId)));
          }
        }
      }
      finally
      {
        manager.Destroy();
      }
    }

    [Test]
    public void DatabaseCreationGrantsPermissionsOnSchema()
    {
      var manager = new PgDbManager(
        new PgDbDescription
        {
          ConnectionInfo = new DbConnectionInfo("ConnectionName"),
          Schemas = new List<DbScript> {new DbScript {ScriptType = ScriptType.Literal, ScriptValue = TestSchema}}
        });

      try
      {
        manager.Create();

        using (var conn = manager.ConnectionInfo.ProviderFactory.CreateConnection())
        {
          conn.ConnectionString = manager.CreateContentConnectionString;
          conn.Open();

          using (var cmd = conn.CreateCommand())
          {
            cmd.CommandText = "SELECT nspacl FROM pg_catalog.pg_namespace WHERE nspname='test_schema'";
            var result = Convert.ToString(cmd.ExecuteScalar());
            Assert.That(result.Contains(string.Format("{0}=UC", manager.userId)));
          }
        }
      }
      finally
      {
        manager.Destroy();
      }
    }

    [Test]
    public void DatabaseSeedSeedsDatabase()
    {
      var manager = new PgDbManager(
        new PgDbDescription
        {
          ConnectionInfo = new DbConnectionInfo("ConnectionName"),
          Schemas = new List<DbScript> {new DbScript {ScriptType = ScriptType.Literal, ScriptValue = TestSchema}},
          Seeds = new List<DbScript> {new DbScript {ScriptType = ScriptType.Literal, ScriptValue = TestSeed}}
        });

      try
      {
        manager.Create();

        manager.Seed();

        using (var conn = manager.ConnectionInfo.ProviderFactory.CreateConnection())
        {
          conn.ConnectionString = manager.CreateContentConnectionString;
          conn.Open();

          using (var cmd = conn.CreateCommand())
          {
            cmd.CommandText = "SELECT COUNT(*) FROM test_schema.test_table";
            Assert.AreEqual(2, Convert.ToInt64(cmd.ExecuteScalar()));
          }
        }
      }
      finally
      {
        manager.Destroy();
      }
    }

    [Test]
    public void NullSuperuserUsesDefaultSuperuser()
    {
      var manager = new PgDbManager(new PgDbDescription {ConnectionInfo = new DbConnectionInfo("ConnectionName")});
      manager.CreateConnectionStrings();
      
      Assert.AreEqual("postgres", new DbConnectionStringBuilder {ConnectionString = manager.CreateDatabaseConnectionString}["database"]);
      Assert.AreEqual("postgres", new DbConnectionStringBuilder {ConnectionString = manager.CreateDatabaseConnectionString}["user id"]);
      Assert.AreEqual("postgres", new DbConnectionStringBuilder {ConnectionString = manager.CreateDatabaseConnectionString}["password"]);
      Assert.AreEqual("utility_database_test", new DbConnectionStringBuilder {ConnectionString = manager.CreateContentConnectionString}["database"]);
      Assert.AreEqual("postgres", new DbConnectionStringBuilder {ConnectionString = manager.CreateContentConnectionString}["user id"]);
      Assert.AreEqual("postgres", new DbConnectionStringBuilder {ConnectionString = manager.CreateContentConnectionString}["password"]);
    }

    [Test]
    public void SpecificSuperuserIsUsed()
    {
      var manager = new PgDbManager(new PgDbDescription {ConnectionInfo = new DbConnectionInfo("ConnectionName")},
                                    new PgSuperuser {Database = "sudb", UserId = "suid", Password = "supw"});
      manager.CreateConnectionStrings();

      Assert.AreEqual("sudb", new DbConnectionStringBuilder {ConnectionString = manager.CreateDatabaseConnectionString}["database"]);
      Assert.AreEqual("suid", new DbConnectionStringBuilder {ConnectionString = manager.CreateDatabaseConnectionString}["user id"]);
      Assert.AreEqual("supw", new DbConnectionStringBuilder {ConnectionString = manager.CreateDatabaseConnectionString}["password"]);
      Assert.AreEqual("utility_database_test", new DbConnectionStringBuilder {ConnectionString = manager.CreateContentConnectionString}["database"]);
      Assert.AreEqual("suid", new DbConnectionStringBuilder {ConnectionString = manager.CreateContentConnectionString}["user id"]);
      Assert.AreEqual("supw", new DbConnectionStringBuilder {ConnectionString = manager.CreateContentConnectionString}["password"]);
    }

    private const string TestSchema = "CREATE SCHEMA test_schema; CREATE TABLE test_schema.test_table ( id serial NOT NULL, name varchar NOT NULL );";
    private const string TestSeed = "INSERT INTO test_schema.test_table (name) VALUES('name1');INSERT INTO test_schema.test_table (name) VALUES('name2');";
  }
}
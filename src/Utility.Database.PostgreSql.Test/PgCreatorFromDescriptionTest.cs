using System;
using System.Data.Common;
using System.Xml.Linq;
using NUnit.Framework;
using Utility.Database.Management.PostgreSql;

namespace Utility.Database.PostgreSql.Test
{
  public class PgCreatorFromDescriptionTest
  {
    [Test]
    public void DatabaseCreationCreatesSchema()
    {
      var manager = new PgDbManager(new PgDbDescription(XElement.Parse(Resources.TestDescription)));

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
    public void DatabaseSeedSeedsDatabase()
    {
      var manager = new PgDbManager(new PgDbDescription(XElement.Parse(Resources.TestDescription)));

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
            Assert.AreEqual(3, Convert.ToInt64(cmd.ExecuteScalar()));
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
      var manager = new PgDbManager(new PgDbDescription(XElement.Parse(Resources.TestDescription)));
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
      var manager = new PgDbManager(new PgDbDescription(XElement.Parse(Resources.TestDescription)),
                                    new PgSuperuser {Database = "sudb", UserId = "suid", Password = "supw"});
      manager.CreateConnectionStrings();

      Assert.AreEqual("sudb", new DbConnectionStringBuilder {ConnectionString = manager.CreateDatabaseConnectionString}["database"]);
      Assert.AreEqual("suid", new DbConnectionStringBuilder {ConnectionString = manager.CreateDatabaseConnectionString}["user id"]);
      Assert.AreEqual("supw", new DbConnectionStringBuilder {ConnectionString = manager.CreateDatabaseConnectionString}["password"]);
      Assert.AreEqual("utility_database_test", new DbConnectionStringBuilder {ConnectionString = manager.CreateContentConnectionString}["database"]);
      Assert.AreEqual("suid", new DbConnectionStringBuilder {ConnectionString = manager.CreateContentConnectionString}["user id"]);
      Assert.AreEqual("supw", new DbConnectionStringBuilder {ConnectionString = manager.CreateContentConnectionString}["password"]);
    }

    [Test]
    public void CreateFromDescriptionWithNoConnectionThrows()
    {
      var manager = new PgDbManager(new PgDbDescription(XElement.Parse(Resources.TestDescriptionNoConnectionName)));

      var e = Assert.Throws<ArgumentException>(manager.Create);
      Assert.AreEqual("Description.Connection", e.ParamName);
    }

    [Test]
    public void DestroyFromDescriptionWithNoConnectionThrows()
    {
      var manager = new PgDbManager(new PgDbDescription(XElement.Parse(Resources.TestDescriptionNoConnectionName)));

      var e = Assert.Throws<ArgumentException>(manager.Destroy);
      Assert.AreEqual("Description.Connection", e.ParamName);
    }

    [Test]
    public void SeedFromDescriptionWithNoConnectionThrows()
    {
      var manager = new PgDbManager(new PgDbDescription(XElement.Parse(Resources.TestDescriptionNoConnectionName)));

      var e = Assert.Throws<ArgumentException>(manager.Seed);
      Assert.AreEqual("Description.Connection", e.ParamName);
    }
  }
}
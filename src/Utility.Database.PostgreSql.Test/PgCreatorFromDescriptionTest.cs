using System;
using System.Xml.Linq;
using NUnit.Framework;

namespace Utility.Database.PostgreSql.Test
{
  public class PgCreatorFromDescriptionTest
  {
    [Test]
    public void DatabaseCreationCreatesSchema()
    {
      var creator = new PgCreator(new PgDbDescription(XElement.Parse(Resources.TestDescription)));

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
    public void DatabaseSeedSeedsDatabase()
    {
      var creator = new PgCreator(new PgDbDescription(XElement.Parse(Resources.TestDescription)));

      try
      {
        creator.Create();

        creator.Seed();

        using (var db = creator.Provider.Database)
        {
          Assert.AreEqual(3, db.ExecuteScalar("SELECT COUNT(*) FROM test_schema.test_table"));
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
      var creator = new PgCreator(new PgDbDescription(XElement.Parse(Resources.TestDescription)));
      creator.CreateProviders();
        
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
      var creator = new PgCreator(new PgDbDescription(XElement.Parse(Resources.TestDescription)),
                                  new PgSuperuser {Database = "sudb", UserId = "suid", Password = "supw"});
      creator.CreateProviders();

      Assert.AreEqual("sudb", creator.CreateDatabaseProvider.ConnectionString["database"]);
      Assert.AreEqual("suid", creator.CreateDatabaseProvider.ConnectionString["user id"]);
      Assert.AreEqual("supw", creator.CreateDatabaseProvider.ConnectionString["password"]);
      Assert.AreEqual("utility_database_test", creator.CreateContentProvider.ConnectionString["database"]);
      Assert.AreEqual("suid", creator.CreateContentProvider.ConnectionString["user id"]);
      Assert.AreEqual("supw", creator.CreateContentProvider.ConnectionString["password"]);
    }

    [Test]
    public void CreateFromDescriptionWithNoConnectionNameThrows()
    {
      var creator = new PgCreator(new PgDbDescription(XElement.Parse(Resources.TestDescriptionNoConnectionName)));

      var e = Assert.Throws<ArgumentException>(creator.Create);
      Assert.AreEqual("Description.ConnectionName", e.ParamName);
    }

    [Test]
    public void DestroyFromDescriptionWithNoConnectionNameThrows()
    {
      var creator = new PgCreator(new PgDbDescription(XElement.Parse(Resources.TestDescriptionNoConnectionName)));

      var e = Assert.Throws<ArgumentException>(creator.Destroy);
      Assert.AreEqual("Description.ConnectionName", e.ParamName);
    }

    [Test]
    public void SeedFromDescriptionWithNoConnectionNameThrows()
    {
      var creator = new PgCreator(new PgDbDescription(XElement.Parse(Resources.TestDescriptionNoConnectionName)));

      var e = Assert.Throws<ArgumentException>(creator.Seed);
      Assert.AreEqual("Description.ConnectionName", e.ParamName);
    }
  }
}
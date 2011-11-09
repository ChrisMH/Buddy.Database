using System.Xml.Linq;
using NUnit.Framework;

namespace Utility.Database.PostgreSql.Test
{
  public class PgCreatorFromDescriptionTest
  {

    [Test]
    public void DatabaseCreationCreatesSchema()
    {
      var creator = new PgCreator(new DbDescription(XElement.Parse(Properties.Resources.TestDescription)));

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
      var creator = new PgCreator(new DbDescription(XElement.Parse(Properties.Resources.TestDescription)));

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
      var creator = new PgCreator(new DbDescription(XElement.Parse(Properties.Resources.TestDescription)));

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
      var creator = new PgCreator(new DbDescription(XElement.Parse(Properties.Resources.TestDescription)),
                                  new PgSuperuser {Database = "sudb", UserId = "suid", Password = "supw"});

      Assert.AreEqual("sudb", creator.CreateDatabaseProvider.ConnectionString["database"]);
      Assert.AreEqual("suid", creator.CreateDatabaseProvider.ConnectionString["user id"]);
      Assert.AreEqual("supw", creator.CreateDatabaseProvider.ConnectionString["password"]);
      Assert.AreEqual("utility_database_test", creator.CreateContentProvider.ConnectionString["database"]);
      Assert.AreEqual("suid", creator.CreateContentProvider.ConnectionString["user id"]);
      Assert.AreEqual("supw", creator.CreateContentProvider.ConnectionString["password"]);
    }
  }
}
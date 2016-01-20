using NUnit.Framework;

namespace Utility.Database.Test
{
  public class DbManagerTest
  {

    [Test]
    public void StaticCreateCreatesDbManager()
    {
      var result = DbManager.Create(DbManagers.DbManager);

      Assert.That(result, Is.Not.Null);
      Assert.That(result, Is.AssignableTo<IDbManager>());
      Assert.That(result, Is.InstanceOf<TestDbManager>());
    }

    [Test]
    public void CreatingADbManagerCreatesTheDbDescription()
    {
      var result = DbManager.Create(DbManagers.DbManager);

      Assert.That(result.Description, Is.Not.Null);
      Assert.That(result.Description, Is.AssignableTo<IDbDescription>());
      Assert.That(result.Description, Is.InstanceOf<TestDbDescription>());
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public void StaticCreateThrowsIfParameterIsInvalid(string xmlRoot)
    {
      Assert.That(() => DbManager.Create(xmlRoot), Throws.ArgumentException.With.Property("ParamName").EqualTo("xmlRoot"));
    }

    [TestCase("<DbManager></DbManager>")]
    [TestCase("<DbManager type=\"\"></DbManager>")]
    [TestCase("<DbManager type=\"InvalidType\"></DbManager>")]
    public void StaticCreateThrowsIfTypeIsInvalid(string xmlRoot)
    {
      Assert.That(() => DbManager.Create(xmlRoot), Throws.ArgumentException.With.Property("ParamName").EqualTo("xmlRoot"));
    }
  }
}
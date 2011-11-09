using System;
using System.Xml.Linq;
using NUnit.Framework;
using System.Linq;

namespace Utility.Database.Test
{
  public class DbDescriptionTest
  {
    [Test]
    public void DescriptionMissingConnectionNameThrows()
    {
      var desc = XElement.Parse(DbDescriptions.Empty);

      var e = Assert.Throws<ArgumentException>(() => new DbDescription(desc));
      Assert.AreEqual("ConnectionName", e.ParamName);
    }

    [Test]
    public void DescriptionWithEmptyConnectionNameThrows()
    {
      var desc = XElement.Parse(DbDescriptions.EmptyConnectionName);

      var e = Assert.Throws<ArgumentException>(() => new DbDescription(desc));
      Assert.AreEqual("ConnectionName", e.ParamName);
    }
    
    [Test]
    public void DescriptionLoadsConnectionName()
    {
      var desc = XElement.Parse(DbDescriptions.MinimumValid);

      var result = new DbDescription(desc);

      Assert.AreEqual("ConnectionName", result.ConnectionName);
    }
    
    [Test]
    public void DescriptionMissingSchemaReturnsEmptySchemaCollection()
    {
      var desc = XElement.Parse(DbDescriptions.MinimumValid);

      var result = new DbDescription(desc);

      Assert.AreEqual(0, result.Schemas.Count());
    }
    
    [Test]
    public void DescriptionMissingSeedReturnsEmptySeedCollection()
    {
      var desc = XElement.Parse(DbDescriptions.MinimumValid);

      var result = new DbDescription(desc);

      Assert.AreEqual(0, result.Seeds.Count());
    }

    [TestCase(DbDescriptions.SingleFileSchema)]
    [TestCase(DbDescriptions.SingleResourceSchema)]
    public void CanLoadSingleSchema(string script)
    {
      var desc = XElement.Parse(script);

      var result = new DbDescription(desc);
      
      Assert.AreEqual(1, result.Schemas.Count());
    }

    [TestCase(DbDescriptions.SingleFileSeed)]
    [TestCase(DbDescriptions.SingleResourceSeed)]
    public void CanLoadSingleSeed(string script)
    {
      var desc = XElement.Parse(script);

      var result = new DbDescription(desc);

      Assert.AreEqual(1, result.Seeds.Count());
    }

    [Test]
    public void CanLoadSchemasAndSeeds()
    {
      var desc = XElement.Parse(DbDescriptions.SchemasAndSeeds);

      var result = new DbDescription(desc);

      Assert.AreEqual(2, result.Schemas.Count());
      Assert.AreEqual(2, result.Seeds.Count());
    }

    [Test]
    public void EmptyBaseDirectoryUsesAppDomainBaseDirectory()
    {
      var root = XElement.Parse(DbDescriptions.SingleFileSchema);

      var result = new DbDescription(root);

      Assert.AreEqual(AppDomain.CurrentDomain.BaseDirectory, result.schemas[0].BaseDirectory);
    }

    [Test]
    public void CanLoadRelativeFileScript()
    {
      var root = XElement.Parse(DbDescriptions.RelativeFileSchema);

      var result = new DbDescription(root, "d:\\DevP\\Utility.Database\\src\\Utility.Database.Test");

      Assert.AreEqual("schema", result.Schemas.First());
      Assert.AreEqual("seed", result.Seeds.First());
    }
  }
}

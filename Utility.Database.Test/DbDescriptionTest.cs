using System;
using System.Xml.Linq;
using NUnit.Framework;
using System.Linq;

namespace Utility.Database.Test
{
  public class DbDescriptionTest
  {
    [Test]
    public void DescriptionMissingConnectionStringThrows()
    {
      var desc = XElement.Parse(DbDescriptions.Empty);

      var e = Assert.Throws<ArgumentException>(() => new DbDescription(desc));
      Assert.AreEqual("ConnectionString", e.ParamName);
    }

    [Test]
    public void DescriptionWithEmptyConnectionStringThrows()
    {
      var desc = XElement.Parse(DbDescriptions.EmptyConnectionString);

      var e = Assert.Throws<ArgumentException>(() => new DbDescription(desc));
      Assert.AreEqual("ConnectionString", e.ParamName);
    }
    
    [Test]
    public void DescriptionMissingProviderNameThrows()
    {
      var desc = XElement.Parse(DbDescriptions.MissingProviderName);

      var e = Assert.Throws<ArgumentException>(() => new DbDescription(desc));
      Assert.AreEqual("ProviderName", e.ParamName);
    }

    [Test]
    public void DescriptionWithEmptyProviderNameThrows()
    {
      var desc = XElement.Parse(DbDescriptions.EmptyProviderName);

      var e = Assert.Throws<ArgumentException>(() => new DbDescription(desc));
      Assert.AreEqual("ProviderName", e.ParamName);
    }

    [Test]
    public void DescriptionParsesConnectionString()
    {
      var desc = XElement.Parse(DbDescriptions.MinimumValid);

      var result = new DbDescription(desc);

      Assert.AreEqual("database=test", result.ConnectionString.ConnectionString);
    }


    [Test]
    public void DescriptionParsesProviderName()
    {
      var desc = XElement.Parse(DbDescriptions.MinimumValid);

      var result = new DbDescription(desc);

      Assert.AreEqual("Provider", result.ProviderName);
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
  }
}

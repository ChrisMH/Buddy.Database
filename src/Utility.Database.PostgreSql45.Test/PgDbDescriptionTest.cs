using System;
using NUnit.Framework;

namespace Utility.Database.PostgreSql.Test
{
  public class PgDbDescriptionTest
  {
    public const string Empty = "<DbDescription></DbDescription>";
    public const string MinimumValidWithInvalidTemplateName = "<DbDescription><TemplateName></TemplateName></DbDescription>";
    public const string MinimumValidWithTemplateName = "<DbDescription><TemplateName>template_postgis</TemplateName></DbDescription>";

    [Test]
    public void DescriptionMissingTemplateNameReturnsNullTemplateName()
    {
      var result = new PgDbDescription {XmlRoot = Empty};

      Assert.Null(result.TemplateName);
    }

    [Test]
    public void DescriptionMissingTemplateNameThrows()
    {
      var e = Assert.Throws<ArgumentException>(() => new PgDbDescription {XmlRoot = MinimumValidWithInvalidTemplateName});

      Assert.AreEqual("TemplateName", e.ParamName);
    }

    [Test]
    public void DescriptionLoadsTemplateName()
    {
      var result = new PgDbDescription {XmlRoot = MinimumValidWithTemplateName};

      Assert.AreEqual("template_postgis", result.TemplateName);
    }
  }
}
using System;
using System.Xml.Linq;
using NUnit.Framework;
using Utility.Database.PostgreSql;

namespace Utility.Database.Test
{
  public class PgDbDescriptionTest
  {
    [Test]
    public void DescriptionMissingTemplateNameReturnsNullTemplateName()
    {
      var desc = XElement.Parse(DbDescriptions.Empty);

      var result = new PgDbDescription(desc);

      Assert.Null(result.TemplateName);
    }

    [Test]
    public void DescriptionMissingTemplateNameThrows()
    {
      var desc = XElement.Parse(DbDescriptions.PgMinimumValidWithInvalidTemplateName);

      var e = Assert.Throws<ArgumentException>(() => new PgDbDescription(desc));

      Assert.AreEqual("TemplateName", e.ParamName);
    }

    [Test]
    public void DescriptionLoadsTemplateName()
    {
      var desc = XElement.Parse(DbDescriptions.PgMinimumValidWithTemplateName);

      var result = new PgDbDescription(desc);

      Assert.AreEqual("template_postgis", result.TemplateName);
    }
  }
}
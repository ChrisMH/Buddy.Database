using System;
using System.Xml.Linq;
using NUnit.Framework;

namespace Utility.Database.PostgreSql.Test
{
  public class PgDbDescriptionTest
  {
    public const string Empty = "<DbDescription></DbDescription>";
    public const string MinimumValidWithInvalidTemplateName = "<DbDescription><TemplateName></TemplateName></DbDescription>";
    public const string MinimumValidWithTemplateName = "<DbDescription><TemplateName>template_postgis</TemplateName></DbDescription>";

    [Test]
    public void PoolingIsNotAllowedByDefault()
    {
      var result = new PgDbDescription();

      Assert.That(result.AllowPooling == false);
    }

    [Test]
    public void ConnectionStringWithoutPoolingIsUnchangedWhenPoolingIsAllowed()
    {
      var connectionInfo = new DbConnectionInfo {ConnectionString = "schema=schema"};
      var result = new PgDbDescription {ConnectionInfo = connectionInfo, AllowPooling = true};

      Assert.AreEqual("schema=schema", result.ConnectionInfo.ConnectionString);
    }

    [Test]
    public void ConnectionStringWithoutPoolingOffIsUnchangedWhenPoolingIsAllowed()
    {
      var connectionInfo = new DbConnectionInfo {ConnectionString = "schema=schema;pooling=false"};
      var result = new PgDbDescription {ConnectionInfo = connectionInfo, AllowPooling = true};

      Assert.AreEqual("schema=schema;pooling=false", result.ConnectionInfo.ConnectionString);
    }

    [Test]
    public void ConnectionStringWithoutPoolingOnIsUnchangedWhenPoolingIsAllowed()
    {
      var connectionInfo = new DbConnectionInfo {ConnectionString = "schema=schema;pooling=true"};
      var result = new PgDbDescription {ConnectionInfo = connectionInfo, AllowPooling = true};

      Assert.AreEqual("schema=schema;pooling=true", result.ConnectionInfo.ConnectionString);
    }

    [Test]
    public void ConnectionStringWithoutPoolingIsSetToPoolingOffWhenPoolingIsNotAllowed()
    {
      var connectionInfo = new DbConnectionInfo {ConnectionString = "schema=schema"};
      var result = new PgDbDescription {ConnectionInfo = connectionInfo};

      Assert.AreEqual("schema=schema;pooling=false", result.ConnectionInfo.ConnectionString);
    }

    [Test]
    public void ConnectionStringWithPoolingOffIsUnchangedWhenPoolingIsNotAllowed()
    {
      var connectionInfo = new DbConnectionInfo {ConnectionString = "schema=schema;pooling=false"};
      var result = new PgDbDescription {ConnectionInfo = connectionInfo};

      Assert.AreEqual("schema=schema;pooling=false", result.ConnectionInfo.ConnectionString);
    }

    [Test]
    public void ConnectionStringWithPoolingOnIsSetToPoolingOffWhenPoolingIsNotAllowed()
    {
      var connectionInfo = new DbConnectionInfo {ConnectionString = "schema=schema;pooling=true"};
      var result = new PgDbDescription {ConnectionInfo = connectionInfo};

      Assert.AreEqual("schema=schema;pooling=false", result.ConnectionInfo.ConnectionString);
    }

        [Test]
    public void DescriptionMissingTemplateNameReturnsNullTemplateName()
    {
      var desc = XElement.Parse(Empty);

      var result = new PgDbDescription(desc);

      Assert.Null(result.TemplateName);
    }

    [Test]
    public void DescriptionMissingTemplateNameThrows()
    {
      var desc = XElement.Parse(MinimumValidWithInvalidTemplateName);

      var e = Assert.Throws<ArgumentException>(() => new PgDbDescription(desc));

      Assert.AreEqual("TemplateName", e.ParamName);
    }

    [Test]
    public void DescriptionLoadsTemplateName()
    {
      var desc = XElement.Parse(MinimumValidWithTemplateName);

      var result = new PgDbDescription(desc);

      Assert.AreEqual("template_postgis", result.TemplateName);
    }
  }
}
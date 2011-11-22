using System;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using Utility.Database.Management;

namespace Utility.Database.Test
{
  public class DbDescriptionTest
  {
    [Test]
    public void DescriptionWithEmptyConnectionReturnsNullConnection()
    {
      var desc = XElement.Parse(DbDescriptions.Empty);

      var result = new DbDescription(desc);

      Assert.Null(result.ConnectionInfo);
    }

    [TestCase(DbDescriptions.ConnectionWithConnectionStringName)]
    [TestCase(DbDescriptions.ConnectionWithConnectionStringAndProviderName)]
    public void DescriptionWithValidConnectionLoadsConnection(string description)
    {
      var desc = XElement.Parse(description);

      var result = new DbDescription(desc);

      Assert.NotNull(result.ConnectionInfo);
      Assert.AreEqual("server=server", result.ConnectionInfo.ConnectionString);
      Assert.AreEqual("System.Data.SqlClient", result.ConnectionInfo.ProviderName);
      Assert.IsInstanceOf<System.Data.SqlClient.SqlClientFactory>(result.ConnectionInfo.ProviderFactory);
    }

    [TestCase(DbDescriptions.EmptyConnection)]
    [TestCase(DbDescriptions.ConnectionWithInvalidConnectionStringName)]
    [TestCase(DbDescriptions.ConnectionWithConnectionString)]
    [TestCase(DbDescriptions.ConnectionWithProviderName)]
    public void DescriptionWithInvalidConnectionThrows(string description)
    {
      var desc = XElement.Parse(description);

      var result = Assert.Throws<ArgumentException>(() => new DbDescription(desc));
      Assert.AreEqual("Connection", result.ParamName);
      Debug.WriteLine(result.Message);
    }
    
    [Test]
    public void DescriptionMissingSchemaReturnsEmptySchemaCollection()
    {
      var desc = XElement.Parse(DbDescriptions.Empty);

      var result = new DbDescription(desc);

      Assert.AreEqual(0, result.Schemas.Count());
    }

    [Test]
    public void DescriptionMissingSeedReturnsEmptySeedCollection()
    {
      var desc = XElement.Parse(DbDescriptions.Empty);

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

      Assert.AreEqual(AppDomain.CurrentDomain.BaseDirectory, result.Schemas[0].BaseDirectory);
    }

    [Test]
    public void CanLoadRelativeFileScript()
    {
      var root = XElement.Parse(DbDescriptions.RelativeFileSchema);

      var result = new DbDescription(root, "d:\\DevP\\Utility.Database\\src\\Utility.Database.Test");

      Assert.AreEqual("schema", result.Schemas.First().Load());
      Assert.AreEqual("seed", result.Seeds.First().Load());
    }

    [Test]
    public void CopyOfDbConnectionInfoIsUsed()
    {
      var connectionInfo = new DbConnectionInfo {ConnectionString = "schema=schema", ProviderName = "System.Data.SqlClient"};
      var result = new DbDescription {ConnectionInfo = connectionInfo};

      connectionInfo.ConnectionString = "schema=other_schema";

      Assert.AreEqual("schema=schema", result.ConnectionInfo.ConnectionString);
    }
  }
}
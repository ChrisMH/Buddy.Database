using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace Utility.Database.Test
{
  public class DbDescriptionTest
  {
    [Test]
    public void DescriptionWithEmptyConnectionReturnsNullConnection()
    {
      var result = new DbDescription<GenericDbConnectionInfo> {XmlRoot = DbDescriptions.Empty};

      Assert.Null(result.ConnectionInfo);
    }

    [TestCase(DbDescriptions.ConnectionWithConnectionStringName, "server=server", "System.Data.SqlClient", typeof (System.Data.SqlClient.SqlClientFactory))]
    [TestCase(DbDescriptions.ConnectionWithConnectionStringAndProviderName, "server=server", "System.Data.SqlClient", typeof (System.Data.SqlClient.SqlClientFactory))]
    [TestCase(DbDescriptions.ConnectionWithConnectionStringAndProviderType, "server=server",
      "System.Data.SqlClient.SqlClientFactory, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", typeof (System.Data.SqlClient.SqlClientFactory))]
    [TestCase(DbDescriptions.ConnectionWithConnectionString, "server=server", null, null)]
    public void DescriptionWithValidConnectionLoadsConnection(string description, string connectionString, string provider, Type providerFactoryType)
    {
      var result = new DbDescription<GenericDbConnectionInfo> {XmlRoot = description};

      Assert.NotNull(result.ConnectionInfo);
      Assert.AreEqual(connectionString, result.ConnectionInfo.ConnectionString);
      Assert.AreEqual(provider, ((IDbProviderInfo) result.ConnectionInfo).Provider);
      if (providerFactoryType != null)
      {
        Assert.IsInstanceOf(providerFactoryType, ((IDbProviderInfo) result.ConnectionInfo).ProviderFactory);
      }
    }

    [TestCase(DbDescriptions.EmptyConnection)]
    [TestCase(DbDescriptions.ConnectionWithInvalidConnectionStringName)]
    [TestCase(DbDescriptions.ConnectionWithProviderName)]
    public void DescriptionWithInvalidConnectionThrows(string description)
    {
      var result = Assert.Throws<ArgumentException>(() => new DbDescription<GenericDbConnectionInfo> {XmlRoot = description});
      Assert.AreEqual("XmlRoot", result.ParamName);
      Debug.WriteLine(result.Message);
    }

    [Test]
    public void DescriptionMissingSchemaReturnsEmptySchemaCollection()
    {
      var result = new DbDescription<GenericDbConnectionInfo> {XmlRoot = DbDescriptions.Empty};

      Assert.AreEqual(0, result.Schemas.Count());
    }

    [Test]
    public void DescriptionMissingSeedReturnsEmptySeedCollection()
    {
      var result = new DbDescription<GenericDbConnectionInfo> {XmlRoot = DbDescriptions.Empty};

      Assert.AreEqual(0, result.Seeds.Count());
    }

    [TestCase(DbDescriptions.SingleFileSchema)]
    [TestCase(DbDescriptions.SingleResourceSchema)]
    public void CanLoadSingleSchema(string script)
    {
      var result = new DbDescription<GenericDbConnectionInfo> {XmlRoot = script};

      Assert.AreEqual(1, result.Schemas.Count());
    }

    [TestCase(DbDescriptions.SingleFileSeed)]
    [TestCase(DbDescriptions.SingleResourceSeed)]
    public void CanLoadSingleSeed(string script)
    {
      var result = new DbDescription<GenericDbConnectionInfo> {XmlRoot = script};

      Assert.AreEqual(1, result.Seeds.Count());
    }

    [Test]
    public void CanLoadSchemasAndSeeds()
    {
      var result = new DbDescription<GenericDbConnectionInfo> {XmlRoot = DbDescriptions.SchemasAndSeeds};

      Assert.AreEqual(2, result.Schemas.Count());
      Assert.AreEqual(2, result.Seeds.Count());
    }

    [Test]
    public void EmptyBaseDirectoryUsesAppDomainBaseDirectory()
    {
      var result = new DbDescription<GenericDbConnectionInfo> {XmlRoot = DbDescriptions.SingleFileSchema};

      Assert.AreEqual(AppDomain.CurrentDomain.BaseDirectory, result.Schemas[0].GetBaseDirectory.Invoke());
    }

    [Test]
    public void CanLoadRelativeFileScript()
    {
      var result = new DbDescription<GenericDbConnectionInfo>
                   {
                     XmlRoot = DbDescriptions.RelativeFileSchema,
                     BaseDirectory = "d:\\DevP\\Utility.Database\\src\\Utility.Database.Test"
                   };

      Assert.AreEqual("schema", result.Schemas.First().Load());
      Assert.AreEqual("seed", result.Seeds.First().Load());
    }

    [Test]
    public void CopyOfDbConnectionInfoIsUsed()
    {
      var connectionInfo = new GenericDbConnectionInfo {ConnectionString = "schema=schema", Provider = "System.Data.SqlClient"};
      var result = new DbDescription<GenericDbConnectionInfo> {ConnectionInfo = connectionInfo};

      connectionInfo.ConnectionString = "schema=other_schema";

      Assert.AreEqual("schema=schema", result.ConnectionInfo.ConnectionString);
    }
  }
}
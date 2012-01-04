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
      var result = new DbDescription {XmlRoot = DbDescriptions.Empty};

      Assert.That(result.ConnectionInfo, Is.Null);
    }

    [TestCase(DbDescriptions.ConnectionWithConnectionStringName, "server=server", "System.Data.SqlClient", typeof (System.Data.SqlClient.SqlClientFactory))]
    [TestCase(DbDescriptions.ConnectionWithConnectionStringAndProviderName, "server=server", "System.Data.SqlClient", typeof (System.Data.SqlClient.SqlClientFactory))]
    [TestCase(DbDescriptions.ConnectionWithConnectionStringAndProviderType, "server=server",
      "System.Data.SqlClient.SqlClientFactory, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", typeof (System.Data.SqlClient.SqlClientFactory))]
    [TestCase(DbDescriptions.ConnectionWithConnectionString, "server=server", null, null)]
    public void DescriptionWithValidConnectionLoadsConnection(string description, string connectionString, string provider, Type providerFactoryType)
    {
      var result = new DbDescription {XmlRoot = description};

      Assert.That(result.ConnectionInfo, Is.Not.Null);
      Assert.That(result.ConnectionInfo.ConnectionString, Is.EqualTo(connectionString));
      Assert.That(result.ConnectionInfo.Provider, Is.EqualTo(provider));
      if (providerFactoryType != null)
      {
        Assert.That(result.ConnectionInfo.ProviderFactory, Is.InstanceOf(providerFactoryType));
      }
    }

    [TestCase(DbDescriptions.EmptyConnection)]
    [TestCase(DbDescriptions.ConnectionWithInvalidConnectionStringName)]
    [TestCase(DbDescriptions.ConnectionWithProviderName)]
    public void DescriptionWithInvalidConnectionThrows(string description)
    {
      Assert.That(() => new DbDescription {XmlRoot = description},
        Throws.ArgumentException.With.Property("ParamName").EqualTo("XmlRoot"));
    }

    [Test]
    public void DescriptionMissingSchemaReturnsEmptySchemaCollection()
    {
      var result = new DbDescription {XmlRoot = DbDescriptions.Empty};

      Assert.That(result.Schemas.Count(), Is.EqualTo(0));
    }

    [Test]
    public void DescriptionMissingSeedReturnsEmptySeedCollection()
    {
      var result = new DbDescription {XmlRoot = DbDescriptions.Empty};

      Assert.That(result.Seeds.Count(), Is.EqualTo(0));
    }

    [TestCase(DbDescriptions.SingleFileSchema)]
    [TestCase(DbDescriptions.SingleResourceSchema)]
    public void CanLoadSingleSchema(string script)
    {
      var result = new DbDescription {XmlRoot = script};

      Assert.That(result.Schemas.Count(), Is.EqualTo(1));
    }

    [TestCase(DbDescriptions.SingleFileSeed)]
    [TestCase(DbDescriptions.SingleResourceSeed)]
    public void CanLoadSingleSeed(string script)
    {
      var result = new DbDescription {XmlRoot = script};

      Assert.That(result.Seeds.Count(), Is.EqualTo(1));
    }

    [Test]
    public void CanLoadSchemasAndSeeds()
    {
      var result = new DbDescription {XmlRoot = DbDescriptions.SchemasAndSeeds};

      Assert.That(result.Schemas.Count(), Is.EqualTo(2));
      Assert.That(result.Seeds.Count(), Is.EqualTo(2));
    }

    [Test]
    public void EmptyBaseDirectoryUsesAppDomainBaseDirectory()
    {
      var result = new DbDescription {XmlRoot = DbDescriptions.SingleFileSchema};

      Assert.That(result.Schemas[0].GetBaseDirectory(), Is.EqualTo(AppDomain.CurrentDomain.BaseDirectory));
    }

    [Test]
    public void CanLoadRelativeFileScript()
    {
      var result = new DbDescription
                   {
                     XmlRoot = DbDescriptions.RelativeFileSchema,
                     BaseDirectory = "d:\\DevP\\Utility.Database\\src\\Utility.Database.Test"
                   };

      Assert.That(result.Schemas.First().Load(), Is.EqualTo("schema"));
      Assert.That(result.Seeds.First().Load(), Is.EqualTo("seed"));
    }

    [Test]
    public void CopyOfDbConnectionInfoIsUsed()
    {
      var connectionInfo = new DbConnectionInfo {ConnectionString = "schema=schema", Provider = "System.Data.SqlClient"};
      var result = new DbDescription {ConnectionInfo = connectionInfo};

      connectionInfo.ConnectionString = "schema=other_schema";

      Assert.That(result.ConnectionInfo.ConnectionString, Is.EqualTo("schema=schema"));
    }


    [Test]
    public void StaticCreateCreatesDbDescription()
    {
      var result = DbDescription.Create(DbDescriptions.ExplicitDbDescriptionType);

      Assert.That(result, Is.Not.Null);
      Assert.That(result, Is.AssignableTo<IDbDescription>());
      Assert.That(result, Is.InstanceOf<TestDbDescription>());
      Assert.That(result.ConnectionInfo.ConnectionString, Is.EqualTo("server=server"));
    }

    [Test]
    public void StaticCreateCreatesWithDefaultTypeIfTypeIsNotSpecified()
    {
      var result = DbDescription.Create(DbDescriptions.ImplicitDbDescriptionType);

      Assert.That(result, Is.Not.Null);
      Assert.That(result, Is.AssignableTo<IDbDescription>());
      Assert.That(result, Is.InstanceOf<DbDescription>());
      Assert.That(result.ConnectionInfo.ConnectionString, Is.EqualTo("server=server"));
    }


    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public void StaticCreateThrowsIfParameterIsInvalid(string xmlRoot)
    {
      Assert.That(() => DbDescription.Create(xmlRoot), Throws.ArgumentException.With.Property("ParamName").EqualTo("xmlRoot"));
    }

    [TestCase("<DbDescription type=\"\"></DbDescription>")]
    [TestCase("<DbDescription type=\"Utility.Database.InvalidType, Utility.Database\"></DbDescription>")]
    public void StaticCreateThrowsIfTypeIsInvalid(string xmlRoot)
    {
      Assert.That(() => DbDescription.Create(xmlRoot), Throws.ArgumentException.With.Property("ParamName").EqualTo("xmlRoot"));
    }
  }
}
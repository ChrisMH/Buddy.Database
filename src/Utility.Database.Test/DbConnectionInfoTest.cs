using System;
using NUnit.Framework;

namespace Utility.Database.Test
{
  public class DbConnectionInfoTest
  {
    [Test]
    public void CanCreateWithConnectionString()
    {
      var result = new DbConnectionInfo
                   {
                     ConnectionString = "server=server"
                   };

      Assert.That(result, Is.Not.Null);
      Assert.That(result.ConnectionString, Is.EqualTo("server=server"));
      Assert.That(result.Provider, Is.Null);
      Assert.That(result.ProviderFactory, Is.Null);
    }

    [Test]
    public void CanCreateWithConnectionStringAndProviderName()
    {
      var result = new DbConnectionInfo
                   {
                     ConnectionString = "server=server",
                     Provider = "System.Data.SqlClient"
                   };

      Assert.That(result, Is.Not.Null);
      Assert.That(result.ConnectionString, Is.EqualTo("server=server"));
      Assert.That(result.Provider, Is.EqualTo("System.Data.SqlClient"));
      Assert.That(result.ProviderFactory, Is.InstanceOf<System.Data.SqlClient.SqlClientFactory>());
    }

    [Test]
    public void CanCreateWithConnectionStringAndProviderType()
    {
      var result = new DbConnectionInfo
                   {
                     ConnectionString = "server=server",
                     Provider = "System.Data.SqlClient.SqlClientFactory, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
                   };

      Assert.That(result, Is.Not.Null);
      Assert.That(result.ConnectionString, Is.EqualTo("server=server"));
      Assert.That(result.Provider, Is.EqualTo("System.Data.SqlClient.SqlClientFactory, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"));
      Assert.That(result.ProviderFactory, Is.InstanceOf<System.Data.SqlClient.SqlClientFactory>());
    }

    [Test]
    public void GetProviderWithInvalidProviderNameThrows()
    {
      var connectionInfo = new DbConnectionInfo
                           {
                             Provider = "Invalid.Provider.Name"
                           };

      Assert.That(() => { var _ = connectionInfo.ProviderFactory; },
                  Throws.ArgumentException.With.Property("ParamName").EqualTo("Provider"));
    }

    [Test]
    public void CanCreateFromConnectionStringName()
    {
      var result = new DbConnectionInfo {ConnectionStringName = "Valid"};

      Assert.That(result, Is.Not.Null);
      Assert.That(result.ConnectionString, Is.EqualTo("server=server"));
      Assert.That(result.Provider, Is.EqualTo("System.Data.SqlClient"));
      Assert.That(result.ProviderFactory, Is.InstanceOf<System.Data.SqlClient.SqlClientFactory>());
    }

    [Test]
    public void CanCreateFromConnectionStringNameWithBlankProviderName()
    {
      var result = new DbConnectionInfo {ConnectionStringName = "BlankProviderName"};

      Assert.That(result, Is.Not.Null);
      Assert.That(result.ConnectionString, Is.EqualTo("server=server"));
      Assert.That(result.Provider, Is.Null);
      Assert.That(result.ProviderFactory, Is.Null);
    }

    [Test]
    public void CanCreateFromConnectionStringNameWithNoProviderName()
    {
      var result = new DbConnectionInfo {ConnectionStringName = "BlankProviderName"};

      Assert.That(result, Is.Not.Null);
      Assert.That(result.ConnectionString, Is.EqualTo("server=server"));
      Assert.That(result.Provider, Is.Null);
      Assert.That(result.ProviderFactory, Is.Null);
    }

    [Test]
    public void CreateFromConnectionStringNameWithBlankConnectionStringThrows()
    {
      Assert.That(() => new DbConnectionInfo {ConnectionStringName = "BlankConnectionString"},
                  Throws.ArgumentException.With.Property("ParamName").EqualTo("ConnectionString"));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("MissingConnectionStringName")]
    public void CreateFromInvalidConnectionStringNameThrows(string connectionStringName)
    {
      Assert.That(() => new DbConnectionInfo {ConnectionStringName = connectionStringName},
                  Throws.ArgumentException.With.Property("ParamName").EqualTo("ConnectionStringName"));
    }

    [Test]
    public void SpecificConnectionStringNamesAreNull()
    {
      var result = new DbConnectionInfo {ConnectionStringName = "Valid"};

      Assert.That(result.ServerAddress, Is.Null);
      Assert.That(result.ServerPort, Is.Null);
      Assert.That(result.DatabaseName, Is.Null);
      Assert.That(result.UserName, Is.Null);
      Assert.That(result.Password, Is.Null);
    }

    [Test]
    public void CanCreateACopy()
    {
      var connectionInfo = (IDbConnectionInfo)new DbConnectionInfo {ConnectionStringName = "Valid"};

      var result = connectionInfo.Copy();

      Assert.That(result, Is.Not.Null);
      Assert.That(result, Is.InstanceOf<DbConnectionInfo>());
      Assert.That(result, Is.Not.SameAs(connectionInfo));
      Assert.That(result.ConnectionString, Is.EqualTo(connectionInfo.ConnectionString));
      Assert.That(result.Provider, Is.EqualTo(connectionInfo.Provider));
      Assert.That(result.ProviderFactory, Is.InstanceOf(connectionInfo.ProviderFactory.GetType()));
    }
  }
}
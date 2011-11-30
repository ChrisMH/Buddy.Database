using System;
using NUnit.Framework;

namespace Utility.Database.PostgreSql.Test
{
  public class GenericDbConnectionInfoTest
  {
    [Test]
    public void ExposesExpectedInterfaces()
    {
      var result = new GenericDbConnectionInfo();

      Assert.IsInstanceOf<IDbConnectionInfo>(result);
      Assert.IsInstanceOf<IDbProviderInfo>(result);
    }

    [Test]
    public void CanCreateWithConnectionString()
    {
      var result = new GenericDbConnectionInfo
                   {
                     ConnectionString = "server=server"
                   };

      Assert.NotNull(result);
      Assert.AreEqual("server=server", result.ConnectionString);
      Assert.Null(result.Provider);
      Assert.Null(result.ProviderFactory);
    }

    [Test]
    public void CanCreateWithConnectionStringAndProviderName()
    {
      var result = new GenericDbConnectionInfo
                   {
                     ConnectionString = "server=server",
                     Provider = "System.Data.SqlClient"
                   };

      Assert.NotNull(result);
      Assert.AreEqual("server=server", result.ConnectionString);
      Assert.AreEqual("System.Data.SqlClient", result.Provider);
      Assert.IsInstanceOf<System.Data.SqlClient.SqlClientFactory>(result.ProviderFactory);
    }

    [Test]
    public void CanCreateWithConnectionStringAndProviderType()
    {
      var result = new GenericDbConnectionInfo
                   {
                     ConnectionString = "server=server",
                     Provider = "System.Data.SqlClient.SqlClientFactory, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
                   };

      Assert.NotNull(result);
      Assert.AreEqual("server=server", result.ConnectionString);
      Assert.AreEqual("System.Data.SqlClient.SqlClientFactory, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", result.Provider);
      Assert.IsInstanceOf<System.Data.SqlClient.SqlClientFactory>(result.ProviderFactory);
    }

    [Test]
    public void GetProviderWithInvalidProviderNameThrows()
    {
      var connectionInfo = new GenericDbConnectionInfo
                           {
                             Provider = "Invalid.Provider.Name"
                           };

      var result = Assert.Throws<ArgumentException>(() => { var factory = connectionInfo.ProviderFactory; });
      Assert.AreEqual("Provider", result.ParamName);
      Console.WriteLine(result.Message);
    }

    [Test]
    public void CanCreateFromConnectionStringName()
    {
      var result = new GenericDbConnectionInfo {ConnectionStringName = "Valid"};

      Assert.NotNull(result);
      Assert.AreEqual("server=server", result.ConnectionString);
      Assert.AreEqual("System.Data.SqlClient", result.Provider);
      Assert.IsInstanceOf<System.Data.SqlClient.SqlClientFactory>(result.ProviderFactory);
    }

    [Test]
    public void CanCreateFromConnectionStringNameWithBlankProviderName()
    {
      var result = new GenericDbConnectionInfo {ConnectionStringName = "BlankProviderName"};

      Assert.NotNull(result);
      Assert.AreEqual("server=server", result.ConnectionString);
      Assert.AreEqual(string.Empty, result.Provider);
      Assert.Null(result.ProviderFactory);
    }

    [Test]
    public void CanCreateFromConnectionStringNameWithNoProviderName()
    {
      var result = new GenericDbConnectionInfo {ConnectionStringName = "BlankProviderName"};

      Assert.NotNull(result);
      Assert.AreEqual("server=server", result.ConnectionString);
      Assert.AreEqual(string.Empty, result.Provider);
      Assert.Null(result.ProviderFactory);
    }

    [Test]
    public void CanCreateFromConnectionStringNameWithBlankConnectionString()
    {
      var result = new GenericDbConnectionInfo {ConnectionStringName = "BlankConnectionString"};

      Assert.AreEqual("", result.ConnectionString);
      Assert.AreEqual("System.Data.SqlClient", result.Provider);
      Assert.IsInstanceOf<System.Data.SqlClient.SqlClientFactory>(result.ProviderFactory);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("MissingConnectionStringName")]
    public void CreateFromInvalidConnectionStringNameThrows(string connectionStringName)
    {
      var result = Assert.Throws<ArgumentException>(() => new GenericDbConnectionInfo {ConnectionStringName = connectionStringName});
      Assert.AreEqual("ConnectionStringName", result.ParamName);
      Console.WriteLine(result.Message);
    }

    [Test]
    public void SpecificConnectionStringNamesAreNull()
    {
      var result = new GenericDbConnectionInfo {ConnectionStringName = "Valid"};

      Assert.IsNull(result.ServerAddress);
      Assert.IsNull(result.ServerPort);
      Assert.IsNull(result.DatabaseName);
      Assert.IsNull(result.UserName);
      Assert.IsNull(result.Password);
    }
  }
}
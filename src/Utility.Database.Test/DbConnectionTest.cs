using System;
using NUnit.Framework;

namespace Utility.Database.Test
{
  public class DbConnectionTest
  {
    [Test]
    public void CanCreateWithConnectionString()
    {
      var result = (IDbConnectionInfo) new DbConnectionInfo
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
      var result = (IDbConnectionInfo) new DbConnectionInfo
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
      var result = (IDbConnectionInfo) new DbConnectionInfo
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
      var connectionInfo = new DbConnectionInfo
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
      var result = (IDbConnectionInfo) new DbConnectionInfo("Valid");

      Assert.NotNull(result);
      Assert.AreEqual("server=server", result.ConnectionString);
      Assert.AreEqual("System.Data.SqlClient", result.Provider);
      Assert.IsInstanceOf<System.Data.SqlClient.SqlClientFactory>(result.ProviderFactory);
    }

    [Test]
    public void CanCreateFromConnectionStringNameWithBlankProviderName()
    {
      var result = (IDbConnectionInfo) new DbConnectionInfo("BlankProviderName");

      Assert.NotNull(result);
      Assert.AreEqual("server=server", result.ConnectionString);
      Assert.Null(result.Provider);
      Assert.Null(result.ProviderFactory);
    }

    [Test]
    public void CanCreateFromConnectionStringNameWithNoProviderName()
    {
      var result = (IDbConnectionInfo) new DbConnectionInfo("BlankProviderName");

      Assert.NotNull(result);
      Assert.AreEqual("server=server", result.ConnectionString);
      Assert.Null(result.Provider);
      Assert.Null(result.ProviderFactory);
    }

    [Test]
    public void CanCreateFromConnectionStringNameWithBlankConnectionString()
    {
      var result = (IDbConnectionInfo) new DbConnectionInfo("BlankConnectionString");
      
      Assert.AreEqual("", result.ConnectionString);
      Assert.AreEqual("System.Data.SqlClient", result.Provider);
      Assert.IsInstanceOf<System.Data.SqlClient.SqlClientFactory>(result.ProviderFactory);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("MissingConnectionStringName")]
    public void CreateFromInvalidConnectionStringNameThrows(string connectionStringName)
    {
      var result = Assert.Throws<ArgumentException>(() => new DbConnectionInfo(connectionStringName));
      Assert.AreEqual("connectionStringName", result.ParamName);
      Console.WriteLine(result.Message);
    }
  }
}
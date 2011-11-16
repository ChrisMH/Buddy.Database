using System;
using NUnit.Framework;

namespace Utility.Database.Test
{
  public class DbConnectionTest
  {
    [Test]
    public void CanCreateWithConnectionString()
    {
      var result = (IDbConnection) new DbConnection
                                   {
                                     ConnectionString = "server=server"
                                   };

      Assert.NotNull(result);
      Assert.AreEqual("server=server", result.ConnectionString);
      Assert.Null(result.ProviderName);
      Assert.Null(result.ProviderFactory);
    }

    [Test]
    public void CanCreateWithConnectionStringAndProviderName()
    {
      var result = (IDbConnection) new DbConnection
                                   {
                                     ConnectionString = "server=server",
                                     ProviderName = "System.Data.SqlClient"
                                   };

      Assert.NotNull(result);
      Assert.AreEqual("server=server", result.ConnectionString);
      Assert.AreEqual("System.Data.SqlClient", result.ProviderName);
      Assert.IsInstanceOf<System.Data.SqlClient.SqlClientFactory>(result.ProviderFactory);
    }

    [Test]
    public void CreateWithInvalidProviderNameThrows()
    {
      var result = Assert.Throws<ArgumentException>(() => new DbConnection
                                                          {
                                                            ProviderName = "Invalid.Provider.Name"
                                                          });
      Assert.AreEqual("ProviderName", result.ParamName);
      Console.WriteLine(result.Message);
    }

    [Test]
    public void CanCreateFromConnectionStringName()
    {
      var result = (IDbConnection) new DbConnection("Valid");

      Assert.NotNull(result);
      Assert.AreEqual("server=server", result.ConnectionString);
      Assert.AreEqual("System.Data.SqlClient", result.ProviderName);
      Assert.IsInstanceOf<System.Data.SqlClient.SqlClientFactory>(result.ProviderFactory);
    }

    [Test]
    public void CanCreateFromConnectionStringNameWithBlankProviderName()
    {
      var result = (IDbConnection) new DbConnection("BlankProviderName");

      Assert.NotNull(result);
      Assert.AreEqual("server=server", result.ConnectionString);
      Assert.Null(result.ProviderName);
      Assert.Null(result.ProviderFactory);
    }

    [Test]
    public void CanCreateFromConnectionStringNameWithNoProviderName()
    {
      var result = (IDbConnection) new DbConnection("BlankProviderName");

      Assert.NotNull(result);
      Assert.AreEqual("server=server", result.ConnectionString);
      Assert.Null(result.ProviderName);
      Assert.Null(result.ProviderFactory);
    }

    [Test]
    public void CanCreateFromConnectionStringNameWithBlankConnectionString()
    {
      var result = (IDbConnection) new DbConnection("BlankConnectionString");
      
      Assert.AreEqual("", result.ConnectionString);
      Assert.AreEqual("System.Data.SqlClient", result.ProviderName);
      Assert.IsInstanceOf<System.Data.SqlClient.SqlClientFactory>(result.ProviderFactory);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("MissingConnectionStringName")]
    public void CreateFromInvalidConnectionStringNameThrows(string connectionStringName)
    {
      var result = Assert.Throws<ArgumentException>(() => new DbConnection(connectionStringName));
      Assert.AreEqual("connectionStringName", result.ParamName);
      Console.WriteLine(result.Message);
    }
  }
}
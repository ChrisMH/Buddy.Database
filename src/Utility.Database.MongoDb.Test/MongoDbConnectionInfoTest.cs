using System;
using NUnit.Framework;
using Utility.Database.Management.MongoDb;

namespace Utility.Database.MongoDb.Test
{
  public class MongoDbConnectionInfoTest
  {
    [Test]
    public void MongoDbConnectionInfoTestExposesExpectedInterfaces()
    {
      var result = new MongoDbConnectionInfo();

      Assert.IsAssignableFrom<IDbConnectionInfo>(result);
    }

    [Test]
    public void CanCreateWithConnectionString()
    {
      var result = new MongoDbConnectionInfo
                   {
                     ConnectionString = "mongodb://localhost/database/"
                   };

      Assert.NotNull(result);
      Assert.AreEqual("mongodb://localhost/database/", result.ConnectionString);
    }

    /*
    

    [Test]
    public void CanCreateFromConnectionStringName()
    {
      var result = new GenericDbConnectionInfo("Valid");

      Assert.NotNull(result);
      Assert.AreEqual("server=server", result.ConnectionString);
      Assert.AreEqual("System.Data.SqlClient", result.Provider);
      Assert.IsInstanceOf<System.Data.SqlClient.SqlClientFactory>(result.ProviderFactory);
    }
    
    [TestCase(null)]
    [TestCase("")]
    [TestCase("MissingConnectionStringName")]
    public void CreateFromInvalidConnectionStringNameThrows(string connectionStringName)
    {
      var result = Assert.Throws<ArgumentException>(() => new GenericDbConnectionInfo(connectionStringName));
      Assert.AreEqual("connectionStringName", result.ParamName);
      Console.WriteLine(result.Message);
    }

    [Test]
    public void CanGetConnectionStringValueFromIndexer()
    {
      var result = new GenericDbConnectionInfo("Valid");

      Assert.NotNull(result);
      Assert.AreEqual("server", result["server"]);
    } 
     */
  }
 
}
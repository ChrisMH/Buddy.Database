using System;
using NUnit.Framework;

namespace Utility.Database.MongoDb.Test
{
  public class MongoDbConnectionInfoTest
  {
    [Test]
    public void MongoDbConnectionInfoTestExposesExpectedInterfaces()
    {
      var result = new MongoDbConnectionInfo();

      Assert.IsInstanceOf<IDbConnectionInfo>(result);
    }

    [Test]
    public void CanCreateWithConnectionStringName()
    {
      var result = new MongoDbConnectionInfo
                   {
                     ConnectionStringName = "Test1"
                   };

      Assert.NotNull(result);
      Assert.AreEqual("mongodb://localhost/UtilityDatabaseTest1", result.ConnectionString);
    }

    [Test]
    public void CanCreateWithConnectionString()
    {
      var result = new MongoDbConnectionInfo
                   {
                     ConnectionString = "mongodb://localhost/database"
                   };

      Assert.NotNull(result);
      Assert.AreEqual("mongodb://localhost/database", result.ConnectionString);
    }

    [Test]
    public void CanCreateACopy()
    {
      var connectionInfo = (IDbConnectionInfo)new MongoDbConnectionInfo
      {
        ConnectionStringName = "Test1"
      };

      var result = connectionInfo.Copy();

      Assert.NotNull(result);
      Assert.IsInstanceOf<MongoDbConnectionInfo>(result);
      Assert.AreNotSame(connectionInfo, result);
      Assert.AreEqual(connectionInfo.ConnectionString, result.ConnectionString);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    [TestCase("MissingConnectionStringName")]
    public void CreateFromInvalidConnectionStringNameThrows(string connectionStringName)
    {
      var result = Assert.Throws<ArgumentException>(() => new MongoDbConnectionInfo
                                                          {
                                                            ConnectionStringName = connectionStringName
                                                          });
      Assert.AreEqual("ConnectionStringName", result.ParamName);
      Console.WriteLine(result.Message);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("  ")]
    [TestCase("invalid")]
    [TestCase("mongodb:///database/")]
    [TestCase("mongodb://localhost/")]
    [TestCase("mongodb://localhost/database/")]
    public void CreateFromInvalidConnectionStringThrows(string connectionString)
    {
      var result = Assert.Throws<ArgumentException>(() => new MongoDbConnectionInfo
                                                          {
                                                            ConnectionString = connectionString
                                                          });
      Assert.AreEqual("ConnectionString", result.ParamName);
      Console.WriteLine(result.Message);
    }

    [Test]
    public void CanGetMinimumConnectionStringValues()
    {
      var result = new MongoDbConnectionInfo
                   {
                     ConnectionString = "mongodb://localhost/database"
                   };

      Assert.AreEqual("localhost", result.ServerAddress);
      Assert.IsNull(result.ServerPort);
      Assert.AreEqual("database", result.DatabaseName);
      Assert.IsNull(result.UserName);
      Assert.IsNull(result.Password);
    }

    [Test]
    public void CanGetFullConnectionStringValues()
    {
      var result = new MongoDbConnectionInfo
                   {
                     ConnectionString = "mongodb://username:password@localhost:8888/database"
                   };
                   
      Assert.AreEqual("localhost", result.ServerAddress);
      Assert.AreEqual(8888, result.ServerPort);
      Assert.AreEqual("database", result.DatabaseName);
      Assert.AreEqual("username", result.UserName);
      Assert.AreEqual("password", result.Password);
    }
  }
}
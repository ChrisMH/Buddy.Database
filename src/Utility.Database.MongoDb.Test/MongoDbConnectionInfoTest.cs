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
      Assert.AreEqual("mongodb://localhost/UtilityDatabaseTest1/", result.ConnectionString);
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
    public void CanGetMinimumConnectionStringValuesFromIndexer()
    {
      var result = new MongoDbConnectionInfo
                   {
                     ConnectionString = "mongodb://localhost/database/"
                   };

      Assert.IsTrue(result.ContainsKey(MongoDbConnectionInfo.ServerAddressKey));
      Assert.AreEqual("localhost", result[MongoDbConnectionInfo.ServerAddressKey]);

      Assert.IsFalse(result.ContainsKey(MongoDbConnectionInfo.ServerPortKey));

      Assert.IsTrue(result.ContainsKey(MongoDbConnectionInfo.DatabaseNameKey));
      Assert.AreEqual("database", result[MongoDbConnectionInfo.DatabaseNameKey]);

      Assert.IsFalse(result.ContainsKey(MongoDbConnectionInfo.UserNameKey));

      Assert.IsFalse(result.ContainsKey(MongoDbConnectionInfo.PasswordKey));
    }

    [Test]
    public void CanGetFullConnectionStringValuesFromIndexer()
    {
      var result = new MongoDbConnectionInfo
                   {
                     ConnectionString = "mongodb://username:password@localhost:8888/database/"
                   };

      Assert.IsTrue(result.ContainsKey(MongoDbConnectionInfo.ServerAddressKey));
      Assert.AreEqual("localhost", result[MongoDbConnectionInfo.ServerAddressKey]);

      Assert.IsTrue(result.ContainsKey(MongoDbConnectionInfo.ServerPortKey));
      Assert.AreEqual(8888, Convert.ToInt32(result[MongoDbConnectionInfo.ServerPortKey]));

      Assert.IsTrue(result.ContainsKey(MongoDbConnectionInfo.DatabaseNameKey));
      Assert.AreEqual("database", result[MongoDbConnectionInfo.DatabaseNameKey]);

      Assert.IsTrue(result.ContainsKey(MongoDbConnectionInfo.UserNameKey));
      Assert.AreEqual("username", result[MongoDbConnectionInfo.UserNameKey]);

      Assert.IsTrue(result.ContainsKey(MongoDbConnectionInfo.PasswordKey));
      Assert.AreEqual("password", result[MongoDbConnectionInfo.PasswordKey]);
    }
  }
}
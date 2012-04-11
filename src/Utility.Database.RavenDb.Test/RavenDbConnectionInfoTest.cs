using System;
using NUnit.Framework;

namespace Utility.Database.RavenDb.Test
{
  public class RavenDbConnectionInfoTest
  {
    [Test]
    public void RavenDbConnectionInfoTestExposesExpectedInterfaces()
    {
      var result = new RavenDbConnectionInfo();

      Assert.That(result, Is.InstanceOf<IDbConnectionInfo>());
    }

    [Test]
    public void CanCreateWithConnectionStringName()
    {
      var result = new RavenDbConnectionInfo
                   {
                     ConnectionStringName = "Test1"
                   };

      Assert.That(result, Is.Not.Null);
      Assert.That(result.ConnectionString, Is.EqualTo("Url=http://localhost;Database=UtilityDatabaseTest1"));
    }

    [Test]
    public void CanCreateWithConnectionString()
    {
      var result = new RavenDbConnectionInfo
                   {
                     ConnectionString = "Url=http://localhost;Database=database"
                   };


      Assert.That(result, Is.Not.Null);
      Assert.That(result.ConnectionString, Is.EqualTo("Url=http://localhost;Database=database"));
    }

    [Test]
    public void CanCreateACopy()
    {
      var connectionInfo = (IDbConnectionInfo)new RavenDbConnectionInfo
      {
        ConnectionStringName = "Test1"
      };

      var result = connectionInfo.Copy();

      Assert.NotNull(result);
      Assert.IsInstanceOf<RavenDbConnectionInfo>(result);
      Assert.AreNotSame(connectionInfo, result);
      Assert.AreEqual(connectionInfo.ConnectionString, result.ConnectionString);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    [TestCase("MissingConnectionStringName")]
    public void CreateFromInvalidConnectionStringNameThrows(string connectionStringName)
    {
      var result = Assert.Throws<ArgumentException>(() => new RavenDbConnectionInfo
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
    public void CreateFromInvalidConnectionStringThrows(string connectionString)
    {
      Assert.That(() => new RavenDbConnectionInfo { ConnectionString = connectionString },
                  Throws.ArgumentException.With.Property("ParamName").EqualTo("ConnectionString"));
    }

    [Test]
    public void CanGetMinimumConnectionStringValues()
    {
      var result = new RavenDbConnectionInfo
                   {
                     ConnectionString = "Url=http://localhost"
                   };

      Assert.That(result.ServerAddress, Is.EqualTo("http://localhost"));
      Assert.That(result.ServerPort, Is.Null);
      Assert.That(result.DatabaseName, Is.Null);
      Assert.That(result.UserName, Is.Null);
      Assert.That(result.Password, Is.Null);
    }

    [Test]
    public void CanGetFullConnectionStringValues()
    {
      var result = new RavenDbConnectionInfo
                   {
                     ConnectionString = "Url=http://localhost:8080;Database=database;User=user;Password=password"
                   };

      Assert.That(result.ServerAddress, Is.EqualTo("http://localhost:8080"));
      Assert.That(result.DatabaseName, Is.EqualTo("database"));
      Assert.That(result.UserName, Is.EqualTo("user"));
      Assert.That(result.Password, Is.EqualTo("password")); 
    }
  }
}
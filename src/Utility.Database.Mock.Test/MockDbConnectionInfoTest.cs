using System;
using NUnit.Framework;
using Utility.Database.Mock;

namespace Utility.Database.Mock.Test
{
  public class MockDbConnectionInfoTest
  {
    [Test]
    public void MockDbConnectionInfoTestExposesExpectedInterfaces()
    {
      var result = new MockDbConnectionInfo();

      Assert.That(result, Is.InstanceOf<IDbConnectionInfo>());
      Assert.That(result, Is.InstanceOf<IMockDbConnectionInfo>());
    }

    [Test]
    public void CanCreateWithConnectionStringName()
    {
      var result = (IDbConnectionInfo)new MockDbConnectionInfo
                   {
                     ConnectionStringName = "Test1"
                   };

      Assert.That(result, Is.Not.Null);
      Assert.That(result.ConnectionString, Is.EqualTo("database=UtilityDatabaseTest1;database type=Utility.Database.Mock.Test.TestMockDatabase, Utility.Database.Mock.Test"));
      Assert.That(((IMockDbConnectionInfo)result).DatabaseType, Is.EqualTo(typeof(TestMockDatabase)));
    }

    [Test]
    public void CanCreateWithConnectionString()
    {
      var result = (IDbConnectionInfo)new MockDbConnectionInfo
      {
        ConnectionString = "database=UtilityDatabaseTest1;database type=Utility.Database.Mock.Test.TestMockDatabase, Utility.Database.Mock.Test"
      };

      Assert.That(result, Is.Not.Null);
      Assert.That(result.ConnectionString, Is.EqualTo("database=UtilityDatabaseTest1;database type=Utility.Database.Mock.Test.TestMockDatabase, Utility.Database.Mock.Test"));
      Assert.That(((IMockDbConnectionInfo)result).DatabaseType, Is.EqualTo(typeof(TestMockDatabase)));
    }


    [Test]
    public void CanCreateACopy()
    {
      var connectionInfo = (IDbConnectionInfo)new MockDbConnectionInfo
      {
        ConnectionStringName = "Test1"
      };

      var result = connectionInfo.Copy();

      Assert.That(result, Is.Not.Null);
      Assert.That(result, Is.Not.SameAs(connectionInfo));
      Assert.That(result, Is.InstanceOf<MockDbConnectionInfo>());
      Assert.That(result.ConnectionString, Is.EqualTo(connectionInfo.ConnectionString));
      Assert.That(((IMockDbConnectionInfo)result).DatabaseType, Is.EqualTo(((IMockDbConnectionInfo)result).DatabaseType));
    }


    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    [TestCase("MissingConnectionStringName")]
    public void CreateFromInvalidConnectionStringNameThrows(string connectionStringName)
    {
      Assert.That(() => new MockDbConnectionInfo {ConnectionStringName = connectionStringName},
                  Throws.ArgumentException.With.Property("ParamName").EqualTo("ConnectionStringName"));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    [TestCase("invalid")]
    [TestCase("database=")]
    [TestCase("database type=")]
    [TestCase("database=;database type=")]
    [TestCase("database=database;database type=")]
    [TestCase("database=;database type=Utility.Database.Mock.Test.TestMockDatabase, Utility.Database.Mock.Test")]
    public void CreateFromInvalidConnectionStringThrows(string connectionString)
    {
      Assert.That(() => new MockDbConnectionInfo { ConnectionString = connectionString },
                  Throws.ArgumentException.With.Property("ParamName").EqualTo("ConnectionString"));
    }

    [Test]
    public void CanGetConnectionStringValues()
    {
      var result = (IDbConnectionInfo)new MockDbConnectionInfo
      {
        ConnectionStringName = "Test1"
      };

      Assert.That(result.DatabaseName, Is.EqualTo("UtilityDatabaseTest1"));
      Assert.That(((IMockDbConnectionInfo)result).DatabaseType, Is.EqualTo(typeof(TestMockDatabase)));
    }
    
    [Test]
    public void GetUnsupportedParametersThrows()
    {
      var connectionInfo = new MockDbConnectionInfo()
      {
        ConnectionStringName = "Test1"
      };

      Assert.Throws<NotImplementedException>(() => { var _ = connectionInfo.ServerAddress; });
      Assert.Throws<NotImplementedException>(() => { var _ = connectionInfo.ServerPort; });
      Assert.Throws<NotImplementedException>(() => { var _ = connectionInfo.UserName; });
      Assert.Throws<NotImplementedException>(() => { var _ = connectionInfo.Password; });
    }
  }
}
using System;
using NUnit.Framework;

namespace Utility.Database.Mock.Test
{
  public class MockDatabaseProviderTest
  {
    [SetUp]
    public void SetUp()
    {
      MockDatabaseProvider.Reset();
    }

    [TearDown]
    public void TearDown()
    {
      MockDatabaseProvider.Reset();
    }

    [Test]
    public void CreateCreatesDatabase()
    {
      var connectionInfo = new MockDbConnectionInfo
                             {
                               ConnectionStringName = "Test1"
                             };

      var mockDatabase = MockDatabaseProvider.Create(connectionInfo);

      Assert.IsNotNull(mockDatabase);
    }

    [Test]
    public void SubsequentOpenReturnsCreated()
    {
      var connectionInfo = new MockDbConnectionInfo
      {
        ConnectionStringName = "Test1"
      };

      {
        var mockDatabase = MockDatabaseProvider.Create(connectionInfo);
        ((TestMockDatabase) mockDatabase).Table.Add(new TestMockDatabase.Row {Id = 101, RowName = "ThisIsARow"});
      }

      {
        var mockDatabase = MockDatabaseProvider.Open(connectionInfo);
        Assert.AreEqual(101, ((TestMockDatabase)mockDatabase).Table[0].Id);
        Assert.AreEqual("ThisIsARow", ((TestMockDatabase)mockDatabase).Table[0].RowName);
      }
    }

    [Test]
    public void OpenThrowsIfDatabaseTypeDoesNotExist()
    {
      var connectionInfo = new MockDbConnectionInfo
      {
        ConnectionStringName = "Test1"
      };

      var result = Assert.Throws<ArgumentException>(() => MockDatabaseProvider.Open(connectionInfo));
      Assert.AreEqual("connectionInfo", result.ParamName);
      Console.WriteLine(result.Message);
    }

    [Test]
    public void OpenThrowsIfDatabaseNameDoesNotExist()
    {
      var connectionInfo = new MockDbConnectionInfo
      {
        ConnectionStringName = "Test1"
      };
      MockDatabaseProvider.Create(connectionInfo);

      connectionInfo.ConnectionStringName = "Test2";

      var result = Assert.Throws<ArgumentException>(() => MockDatabaseProvider.Open(connectionInfo));
      Assert.AreEqual("connectionInfo", result.ParamName);
      Console.WriteLine(result.Message);
    }


    [Test]
    public void DestroyDestroysDatabase()
    {
      var connectionInfo = new MockDbConnectionInfo
      {
        ConnectionStringName = "Test1"
      };

      MockDatabaseProvider.Create(connectionInfo);

      MockDatabaseProvider.Destroy(connectionInfo);

      Assert.Throws<ArgumentException>(() => MockDatabaseProvider.Open(connectionInfo));
    }


    [Test]
    public void CreateThrowsIfConnectionInfoIsInvalid()
    {
      InvalidConnectionInfoTests(connectionInfo => MockDatabaseProvider.Create(connectionInfo));
    }

    [Test]
    public void DestroyThrowsIfConnectionInfoIsInvalid()
    {
      InvalidConnectionInfoTests(MockDatabaseProvider.Destroy);
    }

    [Test]
    public void OpenThrowsIfConnectionInfoIsInvalid()
    {
      InvalidConnectionInfoTests(connectionInfo => MockDatabaseProvider.Open(connectionInfo));
    }

    private void InvalidConnectionInfoTests(Action<IDbConnectionInfo> method)
    {
      Assert.That(() => method.Invoke(null), Throws.ArgumentException.With.Property("ParamName").EqualTo("connectionInfo"));

      Assert.That(() => method.Invoke(new DbConnectionInfo()), Throws.ArgumentException.With.Property("ParamName").EqualTo("ConnectionString"));
    }
  }
}
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

      Assert.IsInstanceOf<IDbConnectionInfo>(result);
      Assert.IsInstanceOf<IDbMockTypeInfo>(result);
    }

    [Test]
    public void CanCreate()
    {
      var result = new MockDbConnectionInfo
                   {
                     ConnectionStringName = "Test1",
                     MockDatabaseType = typeof(TestMockDatabase)
                   };

      Assert.NotNull(result);
      Assert.AreEqual("Test1", result.ConnectionStringName);
      Assert.AreEqual(typeof(TestMockDatabase), result.MockDatabaseType);
    }

    [Test]
    public void CreateWithConnectionStringThrows()
    {
      Assert.Throws<NotImplementedException>(() => new MockDbConnectionInfo { ConnectionString = "server=server" });
    }

    [Test]
    public void GetParametersThrows()
    {
      var connectionInfo = new MockDbConnectionInfo()
      {
        ConnectionStringName = "Test1",
        MockDatabaseType = typeof(TestMockDatabase)
      };
      Assert.Throws<NotImplementedException>(() => { var _ = connectionInfo.ConnectionString; });
      Assert.Throws<NotImplementedException>(() => { var _ = connectionInfo.ServerAddress; });
      Assert.Throws<NotImplementedException>(() => { var _ = connectionInfo.ServerPort; });
      Assert.Throws<NotImplementedException>(() => { var _ = connectionInfo.DatabaseName; });
      Assert.Throws<NotImplementedException>(() => { var _ = connectionInfo.UserName; });
      Assert.Throws<NotImplementedException>(() => { var _ = connectionInfo.Password; });
    }
  }
}
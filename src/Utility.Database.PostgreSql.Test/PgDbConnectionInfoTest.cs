using System;
using NUnit.Framework;

namespace Utility.Database.PostgreSql.Test
{
  public class PgDbConnectionInfoTest
  {
    [Test]
    public void CanGetConnectionStringValues()
    {
      var result = new PgDbConnectionInfo {ConnectionString = "host=server;port=1;database=databasename;user id=username;password=pwd"};

      Assert.AreEqual("server", result.ServerAddress);
      Assert.AreEqual(1, result.ServerPort);
      Assert.AreEqual("databasename", result.DatabaseName);
      Assert.AreEqual("username", result.UserName);
      Assert.AreEqual("pwd", result.Password);
    }

    [Test]
    public void CanCreateACopy()
    {
      var connectionInfo = (IDbConnectionInfo)new PgDbConnectionInfo { ConnectionStringName = "Test1" };

      var result = connectionInfo.Copy();

      Assert.NotNull(result);
      Assert.IsInstanceOf<PgDbConnectionInfo>(result);
      Assert.AreNotSame(connectionInfo, result);
      Assert.AreEqual(connectionInfo.ConnectionString, result.ConnectionString);
      Assert.AreEqual(connectionInfo.Provider, result.Provider);
      Assert.IsInstanceOf<Npgsql.NpgsqlFactory>(result.ProviderFactory);
    }
  }
}
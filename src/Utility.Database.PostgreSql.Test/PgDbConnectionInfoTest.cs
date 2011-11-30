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
  }
}
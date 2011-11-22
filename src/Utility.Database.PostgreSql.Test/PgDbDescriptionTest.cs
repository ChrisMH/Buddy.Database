using NUnit.Framework;
using Utility.Database.Management.PostgreSql;

namespace Utility.Database.PostgreSql.Test
{
  public class PgDbDescriptionTest
  {
    [Test]
    public void PoolingIsNotAllowedByDefault()
    {
      var result = new PgDbDescription();

      Assert.That(result.AllowPooling == false);
    }

    [Test]
    public void ConnectionStringWithoutPoolingIsUnchangedWhenPoolingIsAllowed()
    {
      var connectionInfo = new DbConnectionInfo {ConnectionString = "schema=schema"};
      var result = new PgDbDescription {ConnectionInfo = connectionInfo, AllowPooling = true};

      Assert.AreEqual("schema=schema", result.ConnectionInfo.ConnectionString);
    }

    [Test]
    public void ConnectionStringWithoutPoolingOffIsUnchangedWhenPoolingIsAllowed()
    {
      var connectionInfo = new DbConnectionInfo {ConnectionString = "schema=schema;pooling=false"};
      var result = new PgDbDescription {ConnectionInfo = connectionInfo, AllowPooling = true};

      Assert.AreEqual("schema=schema;pooling=false", result.ConnectionInfo.ConnectionString);
    }

    [Test]
    public void ConnectionStringWithoutPoolingOnIsUnchangedWhenPoolingIsAllowed()
    {
      var connectionInfo = new DbConnectionInfo {ConnectionString = "schema=schema;pooling=true"};
      var result = new PgDbDescription {ConnectionInfo = connectionInfo, AllowPooling = true};

      Assert.AreEqual("schema=schema;pooling=true", result.ConnectionInfo.ConnectionString);
    }

    [Test]
    public void ConnectionStringWithoutPoolingIsSetToPoolingOffWhenPoolingIsNotAllowed()
    {
      var connectionInfo = new DbConnectionInfo {ConnectionString = "schema=schema"};
      var result = new PgDbDescription {ConnectionInfo = connectionInfo};

      Assert.AreEqual("schema=schema;pooling=false", result.ConnectionInfo.ConnectionString);
    }

    [Test]
    public void ConnectionStringWithPoolingOffIsUnchangedWhenPoolingIsNotAllowed()
    {
      var connectionInfo = new DbConnectionInfo {ConnectionString = "schema=schema;pooling=false"};
      var result = new PgDbDescription {ConnectionInfo = connectionInfo};

      Assert.AreEqual("schema=schema;pooling=false", result.ConnectionInfo.ConnectionString);
    }

    [Test]
    public void ConnectionStringWithPoolingOnIsSetToPoolingOffWhenPoolingIsNotAllowed()
    {
      var connectionInfo = new DbConnectionInfo {ConnectionString = "schema=schema;pooling=true"};
      var result = new PgDbDescription {ConnectionInfo = connectionInfo};

      Assert.AreEqual("schema=schema;pooling=false", result.ConnectionInfo.ConnectionString);
    }
  }
}
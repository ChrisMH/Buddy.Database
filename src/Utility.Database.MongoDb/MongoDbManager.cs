using System;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Utility.Database.MongoDb
{
  public class MongoDbManager : IDbManager
  {
    public void Create()
    {
      Destroy();

      var db = CreateDatabase();
      db.GetCollectionNames(); // Creation doesn't happen until a command is run against the database

      foreach (var schemaDefinition in Description.Schemas)
      {
        var command = new BsonJavaScript(schemaDefinition.Load());
        db.Eval(command);
      }
    }

    public void Destroy()
    {
      var server = CreateServer();
      server.DropDatabase(Description.ConnectionInfo.DatabaseName);
    }

    public void Seed()
    {
      var db = CreateDatabase();

      foreach (var seedDefinition in Description.Seeds)
      {
        var command = new BsonJavaScript(seedDefinition.Load());
        db.Eval(command);
      }
    }

    public IDbDescription Description { get; set; }

    internal MongoServer CreateServer()
    {
      VerifyProperties();
      return MongoServer.Create(Description.ConnectionInfo.ConnectionString);
    }

    internal MongoDatabase CreateDatabase()
    {
      VerifyProperties();
      return MongoDatabase.Create(Description.ConnectionInfo.ConnectionString);
    }

    protected void VerifyProperties()
    {
      if (Description == null) throw new ArgumentException("Description is null", "Description");
      if (Description.ConnectionInfo == null) throw new ArgumentException("ConnectionInfo is null", "Description.ConnectionInfo");
      if (string.IsNullOrWhiteSpace(Description.ConnectionInfo.ConnectionString))
        throw new ArgumentException("Description.ConnectionInfo.ConnectionString not provided", "Description.ConnectionInfo.ConnectionString");

      if (Description.ConnectionInfo.GetType() != typeof (MongoDbConnectionInfo))
      {
        Description.ConnectionInfo = new MongoDbConnectionInfo {ConnectionString = Description.ConnectionInfo.ConnectionString};
      }
    }
  }
}
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.RegularExpressions;
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
      server.DropDatabase((string)Description.ConnectionInfo[MongoDbConnectionInfo.DatabaseNameKey]);
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

    public IDbConnectionInfo ConnectionInfo
    {
      get { return Description == null ? null : Description.ConnectionInfo; }
    }
    
    public MongoDbDescription Description { get; set; }

    internal MongoServer CreateServer()
    {
      CheckPreconditions();
      return MongoServer.Create(Description.ConnectionInfo.ConnectionString);
    }

    internal MongoDatabase CreateDatabase()
    {
      var server = CreateServer();
      return server.GetDatabase((string) Description.ConnectionInfo[MongoDbConnectionInfo.DatabaseNameKey], SafeMode.True);
    }

    protected void CheckPreconditions()
    {
      if(Description == null) throw new ArgumentNullException("Description", "Description is not set");
      if(Description.ConnectionInfo == null) throw new ArgumentNullException("Description.ConnectionInfo", "ConnectionInfo is not set");
     
     
    }

  }
}
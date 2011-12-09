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
      CheckPreconditions();
      return MongoServer.Create(Description.ConnectionInfo.ConnectionString);
    }

    internal MongoDatabase CreateDatabase()
    {
      CheckPreconditions();
      return MongoDatabase.Create(Description.ConnectionInfo.ConnectionString);
    }

    protected void CheckPreconditions()
    {
      if(Description == null) throw new ArgumentNullException("Description", "Description is not set");
      if(Description.ConnectionInfo == null) throw new ArgumentNullException("Description.ConnectionInfo", "ConnectionInfo is not set");
    }

  }
}
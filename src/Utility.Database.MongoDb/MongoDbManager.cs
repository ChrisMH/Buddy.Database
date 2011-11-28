using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Utility.Database.Management.MongoDb
{
  public class MongoDbManager : IDbManager
  {
    private const string ConnectionStringRegex = @"mongodb://((?<UserId>[^:]+):(?<Password>[^@]+)@)?(?<ServerName>[^/]+)/(?<DatabaseName>[^/]+)/?";

    public MongoDbManager(DbDescription description)
    {
      if (description == null) throw new ArgumentNullException("description");
      Description = description;
    }

    public void Create()
    {
      var connectionParams = ParseConnectionString(Description.ConnectionInfo);

      Destroy();

      var server = MongoServer.Create(connectionParams.ConnectionString);

      var db = server.GetDatabase(connectionParams.DatabaseName, SafeMode.True);
      db.GetCollectionNames(); // Creation doesn't happen until a command is run against the database

      foreach (var schemaDefinition in Description.Schemas)
      {
        var command = new BsonJavaScript(schemaDefinition.Load());
        db.Eval(command);
      }
    }

    public void Destroy()
    {
      var connectionParams = ParseConnectionString(Description.ConnectionInfo);
      
      var server = MongoServer.Create(connectionParams.ConnectionString);
      server.DropDatabase(connectionParams.DatabaseName);
    }

    public void Seed()
    {
      var connectionParams = ParseConnectionString(Description.ConnectionInfo);
      
      var server = MongoServer.Create(connectionParams.ConnectionString);
      var db = server.GetDatabase(connectionParams.DatabaseName, SafeMode.True);
      
      foreach (var seedDefinition in Description.Seeds)
      {
        var command = new BsonJavaScript(seedDefinition.Load());
        db.Eval(command);
      }

    }

    public IDbConnectionInfo ConnectionInfo
    {
      get { return Description.ConnectionInfo; }
    }

    public DbDescription Description { get; private set; }

    internal static dynamic ParseConnectionString(IDbConnectionInfo connectionInfo)
    {
      if (connectionInfo == null) throw new ArgumentNullException("connectionInfo");
      if (string.IsNullOrEmpty(connectionInfo.ConnectionString)) throw new ArgumentException("Connection information is missing a connection string", "connectionInfo.ConnectionString");

      var matched = Regex.Match(connectionInfo.ConnectionString, ConnectionStringRegex);
      if (!matched.Success)
      {
        throw new ArgumentException(string.Format("Connection string is not valid: '{0}'.  A minimal connection string has the format: 'mongodb://<server>/<database>/'.  Note that the database name is required.",
                                                  connectionInfo.ConnectionString),
                                    "connectionInfo.ConnectionString");
      }

      var result = new ExpandoObject() as IDictionary<string, object>;
      result["ConnectionString"] = connectionInfo.ConnectionString;
      result["ServerName"] = matched.Groups["ServerName"].Value;
      result["DatabaseName"] = matched.Groups["DatabaseName"].Value;
      result["UserId"] = matched.Groups["UserId"] == null ? null : matched.Groups["UserId"].Value;
      result["Password"] = matched.Groups["Password"] == null ? null : matched.Groups["Password"].Value;

      return result;
    }

  }
}
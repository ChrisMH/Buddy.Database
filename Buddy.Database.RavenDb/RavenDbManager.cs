using System;
using Raven.Client;
using Raven.Client.Document;

namespace Utility.Database.RavenDb
{
  public class RavenDbManager : IDbManager
  {
    public void Create()
    {
      Destroy();

      using(var db = CreateDatabase())
      {

      }
    }

    public void Destroy()
    {
      // There is no way to programmatically destroy a RavenDB database
    }

    public void Seed()
    {
      var db = CreateDatabase();

      //foreach (var seedDefinition in Description.Seeds)
      //{
      //  var command = new BsonJavaScript(seedDefinition.Load());
      //  db.Eval(command);
      //}
    }

    public IDbDescription Description { get; set; }
    
    internal IDocumentSession CreateDatabase()
    {
      VerifyProperties();

      var documentStore = new DocumentStore { Url = Description.ConnectionInfo.ServerAddress, DefaultDatabase = Description.ConnectionInfo.DatabaseName };
      documentStore.Initialize();
      
      return documentStore.OpenSession();
    }

    protected void VerifyProperties()
    {
      if (Description == null) throw new ArgumentException("Description is null", "Description");
      if (Description.ConnectionInfo == null) throw new ArgumentException("ConnectionInfo is null", "Description.ConnectionInfo");
      if (string.IsNullOrWhiteSpace(Description.ConnectionInfo.ConnectionString))
        throw new ArgumentException("Description.ConnectionInfo.ConnectionString not provided", "Description.ConnectionInfo.ConnectionString");

      if (Description.ConnectionInfo.GetType() != typeof (RavenDbConnectionInfo))
      {
        Description.ConnectionInfo = new RavenDbConnectionInfo { ConnectionString = Description.ConnectionInfo.ConnectionString };
      }
    }
  }
}
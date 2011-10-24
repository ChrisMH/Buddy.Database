using System;
using System.Collections.Generic;
using MicrOrm;
using MicrOrm.Core;

namespace Utility.Database.PostgreSql
{
  public class DbCreator : IDbCreator
  {
    public DbCreator(string connectionStringName, Func<IEnumerable<string>> schemaDefinitions, Func<IEnumerable<string>> seedDefinitions)
    {
      Provider = new MoConnectionProvider(connectionStringName);
      Provider.ConnectionString.Remove("schema");

      CreateProvider = new MoConnectionProvider(connectionStringName);
      CreateProvider.ConnectionString.Remove("schema");
      CreateProvider.ConnectionString["database"] = "postgres";

      SchemaDefinitions = schemaDefinitions;
      SeedDefinitions = seedDefinitions;
    }


    public void Create()
    {
      Destroy();
      using (var db = CreateProvider.Database)
      {
        db.ExecuteNonQuery(string.Format("CREATE DATABASE {0}", Provider.ConnectionString["database"]));
      }

      if (SchemaDefinitions == null) return;

      using (var db = Provider.Database)
      {
        foreach (var schemaDefinition in SchemaDefinitions.Invoke())
        {
          db.ExecuteNonQuery(schemaDefinition);
        }
      }
    }

    public void Destroy()
    {
      using (var db = CreateProvider.Database)
      {
        db.ExecuteNonQuery(String.Format("DROP DATABASE IF EXISTS {0}", Provider.ConnectionString["database"]));
      }
    }

    public void Seed()
    {
      if (SeedDefinitions == null) return;

      using (var db = Provider.Database)
      {
        foreach (var seedDefinition in SeedDefinitions.Invoke())
        {
          db.ExecuteNonQuery(seedDefinition);
        }
      }
    }

    public IMoConnectionProvider Provider { get; private set; }

    private IMoConnectionProvider CreateProvider { get; set; }
    private Func<IEnumerable<string>> SchemaDefinitions { get; set; }
    private Func<IEnumerable<string>> SeedDefinitions { get; set; } 
  }
}
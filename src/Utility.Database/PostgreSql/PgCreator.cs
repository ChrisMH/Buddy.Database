using System;
using System.Collections.Generic;
using System.Linq;
using MicrOrm;
using MicrOrm.Core;

namespace Utility.Database.PostgreSql
{
  public class PgCreator : IDbCreator
  {
    public PgCreator(string connectionName,
                     Func<IEnumerable<string>> schemaDefinitions = null,
                     Func<IEnumerable<string>> seedDefinitions = null,
                     PgSuperuser superuser = null)
    {
      CreateProviders(connectionName, superuser ?? new PgSuperuser());

      SchemaDefinitions = schemaDefinitions;
      SeedDefinitions = seedDefinitions;
    }


    public PgCreator(DbDescription description,
                     PgSuperuser superuser = null)
    {
      CreateProviders(description.ConnectionName, superuser ?? new PgSuperuser());

      SchemaDefinitions = () => description.Schemas;
      SeedDefinitions = () => description.Seeds;
    }


    public void Create()
    {
      Destroy();
      using (var db = CreateDatabaseProvider.Database)
      {
        db.ExecuteNonQuery(string.Format("CREATE DATABASE {0}", Provider.ConnectionString["database"]));

        if (Provider.ConnectionString.ContainsKey("user id") && Provider.ConnectionString.ContainsKey("password"))
        {
          if (db.ExecuteScalar("SELECT COUNT(*) FROM pg_catalog.pg_user WHERE usename=:p0", Provider.ConnectionString["user id"]) == 0)
          {
            db.ExecuteNonQuery(string.Format("CREATE ROLE {0} LOGIN ENCRYPTED PASSWORD '{1}' NOINHERIT", Provider.ConnectionString["user id"], Provider.ConnectionString["password"]));
            createdUser = true;
          }
        }
      }

      if (SchemaDefinitions == null) return;

      using (var db = CreateContentProvider.Database)
      {
        foreach (var schemaDefinition in SchemaDefinitions.Invoke())
        {
          db.ExecuteNonQuery(schemaDefinition);
        }

        var schemas = db.ExecuteReader("SELECT nspname FROM pg_catalog.pg_namespace " +
                                       "WHERE nspname NOT LIKE 'pg_%' " +
                                       "AND nspname NOT IN ('information_schema')")
          .Select(s => s.Nspname)
          .ToList();
        foreach (var schema in schemas)
        {
          db.ExecuteNonQuery(string.Format("GRANT ALL ON SCHEMA {0} TO {1}", schema, Provider.ConnectionString["user id"]));
          db.ExecuteNonQuery(string.Format("GRANT ALL ON ALL TABLES IN SCHEMA {0} TO {1}", schema, Provider.ConnectionString["user id"]));
          db.ExecuteNonQuery(string.Format("GRANT ALL ON ALL SEQUENCES IN SCHEMA {0} TO {1}", schema, Provider.ConnectionString["user id"]));
          db.ExecuteNonQuery(string.Format("GRANT ALL ON ALL FUNCTIONS IN SCHEMA {0} TO {1}", schema, Provider.ConnectionString["user id"]));
        }
      }
    }

    public void Destroy()
    {
      using (var db = CreateDatabaseProvider.Database)
      {
        db.ExecuteNonQuery(string.Format("DROP DATABASE IF EXISTS {0}", Provider.ConnectionString["database"]));

        if (createdUser)
        {
          db.ExecuteNonQuery(string.Format("DROP ROLE {0}", Provider.ConnectionString["user id"]));
        }
      }
    }

    public void Seed()
    {
      if (SeedDefinitions == null) return;

      using (var db = CreateContentProvider.Database)
      {
        foreach (var seedDefinition in SeedDefinitions.Invoke())
        {
          db.ExecuteNonQuery(seedDefinition);
        }
      }
    }

    private void CreateProviders(string connectionName, PgSuperuser superuser)
    {
      Provider = new MoConnectionProvider(connectionName);
      Provider.ConnectionString.Remove("schema");

      CreateDatabaseProvider = new MoConnectionProvider(connectionName);
      CreateDatabaseProvider.ConnectionString.Remove("schema");
      CreateDatabaseProvider.ConnectionString["database"] = superuser.Database;
      CreateDatabaseProvider.ConnectionString["user id"] = superuser.UserId;
      CreateDatabaseProvider.ConnectionString["password"] = superuser.Password;

      CreateContentProvider = new MoConnectionProvider(connectionName);
      CreateContentProvider.ConnectionString.Remove("schema");
      CreateContentProvider.ConnectionString["user id"] = superuser.UserId;
      CreateContentProvider.ConnectionString["password"] = superuser.Password;
    }

    public IMoConnectionProvider Provider { get; private set; }

    internal IMoConnectionProvider CreateDatabaseProvider { get; set; }
    internal IMoConnectionProvider CreateContentProvider { get; set; }
    internal Func<IEnumerable<string>> SchemaDefinitions { get; set; }
    internal Func<IEnumerable<string>> SeedDefinitions { get; set; }

    private bool createdUser;
  }
}
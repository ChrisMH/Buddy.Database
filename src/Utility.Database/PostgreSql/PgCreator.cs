using System;
using System.Collections.Generic;
using System.Linq;
using MicrOrm;
using MicrOrm.Core;

namespace Utility.Database.PostgreSql
{
  public class PgCreator : IDbCreator
  {
    public PgCreator(PgDbDescription description)
      : this(description, new PgSuperuser())
    {
    }

    public PgCreator(PgDbDescription description, PgSuperuser superuser)
    {
      createProviders = CreateProviders;
      if (description == null) throw new ArgumentNullException("description");
      
      Description = description;
      Superuser = superuser ?? new PgSuperuser();
    }

    public void Create()
    {
      createProviders();

      Destroy();
      using (var db = CreateDatabaseProvider.Database)
      {
        var createDatabaseCommand = string.Format("CREATE DATABASE \"{0}\"", Provider.ConnectionString["database"]);
        if(!string.IsNullOrEmpty(Description.TemplateName))
        {
          createDatabaseCommand += string.Format(" TEMPLATE \"{0}\"", Description.TemplateName);
        }
        db.ExecuteNonQuery(createDatabaseCommand);

        if (Provider.ConnectionString.ContainsKey("user id") && Provider.ConnectionString.ContainsKey("password"))
        {
          if (db.ExecuteScalar("SELECT COUNT(*) FROM pg_catalog.pg_user WHERE usename=:p0", Provider.ConnectionString["user id"]) == 0)
          {
            db.ExecuteNonQuery(string.Format("CREATE ROLE \"{0}\" LOGIN ENCRYPTED PASSWORD '{1}' NOINHERIT", Provider.ConnectionString["user id"], Provider.ConnectionString["password"]));
            createdUser = true;
          }
        }
      }
      
      using (var db = CreateContentProvider.Database)
      {
        foreach (var schemaDefinition in Description.Schemas)
        {
          db.ExecuteNonQuery(schemaDefinition.Load());
        }

        var schemas = db.ExecuteReader("SELECT nspname FROM pg_catalog.pg_namespace " +
                                       "WHERE nspname NOT LIKE 'pg_%' " +
                                       "AND nspname NOT IN ('information_schema')")
          .Select(s => s.Nspname)
          .ToList();
        foreach (var schema in schemas)
        {
          db.ExecuteNonQuery(string.Format("GRANT ALL ON SCHEMA \"{0}\" TO \"{1}\"", schema, Provider.ConnectionString["user id"]));
          db.ExecuteNonQuery(string.Format("GRANT ALL ON ALL TABLES IN SCHEMA \"{0}\" TO \"{1}\"", schema, Provider.ConnectionString["user id"]));
          db.ExecuteNonQuery(string.Format("GRANT ALL ON ALL SEQUENCES IN SCHEMA \"{0}\" TO \"{1}\"", schema, Provider.ConnectionString["user id"]));
          db.ExecuteNonQuery(string.Format("GRANT ALL ON ALL FUNCTIONS IN SCHEMA \"{0}\" TO \"{1}\"", schema, Provider.ConnectionString["user id"]));
        }
      }
    }

    public void Destroy()
    {
      createProviders();

      using (var db = CreateDatabaseProvider.Database)
      {
        db.ExecuteNonQuery(string.Format("DROP DATABASE IF EXISTS \"{0}\"", Provider.ConnectionString["database"]));

        if (createdUser)
        {
          db.ExecuteNonQuery(string.Format("DROP ROLE \"{0}\"", Provider.ConnectionString["user id"]));
        }
      }
    }

    public void Seed()
    {
      createProviders();

      using (var db = CreateContentProvider.Database)
      {
        foreach (var seedDefinition in Description.Seeds)
        {
          db.ExecuteNonQuery(seedDefinition.Load());
        }
      }
    }

    internal void CreateProviders()
    {
      if(string.IsNullOrEmpty(Description.ConnectionName)) throw new ArgumentException("ConnectionName is empty", "Description.ConnectionName");

      Provider = new MoConnectionProvider(Description.ConnectionName);
      Provider.ConnectionString.Remove("schema");

      CreateDatabaseProvider = new MoConnectionProvider(Description.ConnectionName);
      CreateDatabaseProvider.ConnectionString.Remove("schema");
      CreateDatabaseProvider.ConnectionString["database"] = Superuser.Database;
      CreateDatabaseProvider.ConnectionString["user id"] = Superuser.UserId;
      CreateDatabaseProvider.ConnectionString["password"] = Superuser.Password;

      CreateContentProvider = new MoConnectionProvider(Description.ConnectionName);
      CreateContentProvider.ConnectionString.Remove("schema");
      CreateContentProvider.ConnectionString["user id"] = Superuser.UserId;
      CreateContentProvider.ConnectionString["password"] = Superuser.Password;

      createProviders = () => { }; // Done, so don't call again
    }

    public IMoConnectionProvider Provider { get; private set; }
    public PgDbDescription Description { get; private set; }
    public PgSuperuser Superuser { get; private set; }

    internal IMoConnectionProvider CreateDatabaseProvider { get; set; }
    internal IMoConnectionProvider CreateContentProvider { get; set; }

    private Action createProviders;
    private bool createdUser;
  }
}
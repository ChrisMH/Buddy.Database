using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Utility.Database.Management.PostgreSql
{
  public class PgDbManager : IDbManager
  {
    public PgDbManager(PgDbDescription description)
      : this(description, new PgSuperuser())
    {
    }

    public PgDbManager(PgDbDescription description, PgSuperuser superuser)
    {
      createConnectionStrings = CreateConnectionStrings;
      if (description == null) throw new ArgumentNullException("description");

      Description = description;
      Superuser = superuser ?? new PgSuperuser();
    }

    public void Create()
    {
      createConnectionStrings();

      Destroy();
      using (var conn = ConnectionInfo.ProviderFactory.CreateConnection())
      {
        conn.ConnectionString = CreateDatabaseConnectionString;
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandText = string.Format("CREATE DATABASE \"{0}\"", databaseName);
          if (!string.IsNullOrEmpty(Description.TemplateName))
          {
            cmd.CommandText += string.Format(" TEMPLATE \"{0}\"", Description.TemplateName);
          }
          cmd.ExecuteNonQuery();

          if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(password))
          {
            cmd.CommandText = string.Format("SELECT COUNT(*) FROM pg_catalog.pg_user WHERE usename='{0}'", userId);
            if (Convert.ToInt64(cmd.ExecuteScalar()) == 0)
            {
              cmd.CommandText = string.Format("CREATE ROLE \"{0}\" LOGIN ENCRYPTED PASSWORD '{1}' NOINHERIT", userId, password);
              createdUser = true;
            }
          }
        }
      }

      using (var conn = ConnectionInfo.ProviderFactory.CreateConnection())
      {
        conn.ConnectionString = CreateContentConnectionString;
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          foreach (var schemaDefinition in Description.Schemas)
          {
            cmd.CommandText = schemaDefinition.Load();
            cmd.ExecuteNonQuery();
          }

          if (!string.IsNullOrEmpty(userId))
          {
            cmd.CommandText = "SELECT nspname FROM pg_catalog.pg_namespace " +
                              "WHERE nspname NOT LIKE 'pg_%' " +
                              "AND nspname NOT IN ('information_schema')";
            var schemas = new List<string>();
            using (var rdr = cmd.ExecuteReader())
            {
              while (rdr.Read())
              {
                schemas.Add(rdr.GetString(0));
              }
            }

            if (schemas.Count > 0)
            {
              cmd.CommandText = "";
              foreach (var schema in schemas)
              {
                cmd.CommandText += string.Format("GRANT ALL ON SCHEMA \"{0}\" TO \"{1}\";" +
                                                 "GRANT ALL ON ALL TABLES IN SCHEMA \"{0}\" TO \"{1}\";" +
                                                 "GRANT ALL ON ALL SEQUENCES IN SCHEMA \"{0}\" TO \"{1}\";" +
                                                 "GRANT ALL ON ALL FUNCTIONS IN SCHEMA \"{0}\" TO \"{1}\";",
                                                 schema, userId);
              }

              cmd.ExecuteNonQuery();
            }
          }
        }
      }
    }

    public void Destroy()
    {
      createConnectionStrings();

      using (var conn = ConnectionInfo.ProviderFactory.CreateConnection())
      {
        conn.ConnectionString = CreateDatabaseConnectionString;
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandText = string.Format("DROP DATABASE IF EXISTS \"{0}\"", databaseName);
          cmd.ExecuteNonQuery();

          if (createdUser)
          {
            cmd.CommandText = string.Format("DROP ROLE \"{0}\"", userId);
            cmd.ExecuteNonQuery();
          }
        }
      }
    }

    public void Seed()
    {
      createConnectionStrings();

      using (var conn = ConnectionInfo.ProviderFactory.CreateConnection())
      {
        conn.ConnectionString = CreateContentConnectionString;
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          foreach (var seedDefinition in Description.Seeds)
          {
            cmd.CommandText = seedDefinition.Load();
            cmd.ExecuteNonQuery();
          }
        }
      }
    }

    public IDbConnectionInfo ConnectionInfo
    {
      get { return Description.ConnectionInfo; }
    }

    internal void CreateConnectionStrings()
    {
      if (Description.ConnectionInfo == null) throw new ArgumentException("Connection information is missing", "Description.Connection");
      if (string.IsNullOrEmpty(Description.ConnectionInfo.ConnectionString)) throw new ArgumentException("Connection information is missing a connection string", "Description.Connection.ConnectionString");
      if (Description.ConnectionInfo.ProviderFactory == null) throw new ArgumentException("Connection information is missing a provider factory", "Description.Connection.ProviderFactory");

      var csBuilder = new DbConnectionStringBuilder {ConnectionString = Description.ConnectionInfo.ConnectionString};
      databaseName = Convert.ToString(csBuilder["database"]);
      userId = Convert.ToString(csBuilder["user id"]);
      password = Convert.ToString(csBuilder["password"]);

      // Database/user creation connection string
      csBuilder["database"] = Superuser.Database;
      csBuilder["user id"] = Superuser.UserId;
      csBuilder["password"] = Superuser.Password;
      CreateDatabaseConnectionString = csBuilder.ConnectionString;

      // Database content creation connection string
      csBuilder = new DbConnectionStringBuilder {ConnectionString = Description.ConnectionInfo.ConnectionString};
      csBuilder["user id"] = Superuser.UserId;
      csBuilder["password"] = Superuser.Password;
      CreateContentConnectionString = csBuilder.ConnectionString;

      // Done, so don't create again
      createConnectionStrings = () => { };
    }

    public PgDbDescription Description { get; private set; }
    public PgSuperuser Superuser { get; private set; }

    internal string CreateDatabaseConnectionString { get; set; }
    internal string CreateContentConnectionString { get; set; }

    internal string databaseName;
    internal string userId;
    internal string password;
    internal bool createdUser;

    private Action createConnectionStrings;
  }
}
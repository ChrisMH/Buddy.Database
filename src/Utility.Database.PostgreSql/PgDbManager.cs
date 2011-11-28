using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Utility.Database.PostgreSql
{
  public class PgDbManager : IDbManager
  {
    internal const string DatabaseKey = "database";
    internal const string UserIdKey = "user id";
    internal const string PasswordKey = "password";

    public PgDbManager(PgDbDescription description)
      : this(description, new PgSuperuser())
    {
    }

    public PgDbManager(PgDbDescription description, PgSuperuser superuser)
    {
      if (description == null) throw new ArgumentNullException("description");

      Description = description;
      Superuser = superuser ?? new PgSuperuser();
    }

    public void Create()
    {
      if (Description.ConnectionInfo == null) throw new ArgumentNullException("Description.ConnectionInfo");
      if (string.IsNullOrEmpty(Description.ConnectionInfo.ConnectionString)) throw new ArgumentException("Connection information is missing a connection string", "Description.ConnectionInfo.ConnectionString");
      if (Description.ConnectionInfo.ProviderFactory == null) throw new ArgumentException("Connection information is missing a provider factory", "Description.ConnectionInfo.ProviderFactory");
      
      Destroy();
      
      var csBuilder = new DbConnectionStringBuilder {ConnectionString = Description.ConnectionInfo.ConnectionString};

      using (var conn = ConnectionInfo.ProviderFactory.CreateConnection())
      {
        conn.ConnectionString = CreateDatabaseConnectionString(Description.ConnectionInfo, Superuser);
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {

          cmd.CommandText = string.Format("CREATE DATABASE \"{0}\"", csBuilder[DatabaseKey]);
          if (!string.IsNullOrEmpty(Description.TemplateName))
          {
            cmd.CommandText += string.Format(" TEMPLATE \"{0}\"", Description.TemplateName);
          }
          cmd.ExecuteNonQuery();

          if (csBuilder.ContainsKey(UserIdKey) && csBuilder.ContainsKey(PasswordKey))
          {
            cmd.CommandText = string.Format("SELECT COUNT(*) FROM pg_catalog.pg_user WHERE usename='{0}'", csBuilder[UserIdKey]);
            if (Convert.ToInt64(cmd.ExecuteScalar()) == 0)
            {
              cmd.CommandText = string.Format("CREATE ROLE \"{0}\" LOGIN ENCRYPTED PASSWORD '{1}' NOINHERIT", csBuilder[UserIdKey], csBuilder[PasswordKey]);
              cmd.ExecuteNonQuery();
            }
          }
        }
      }

      using (var conn = ConnectionInfo.ProviderFactory.CreateConnection())
      {
        conn.ConnectionString = CreateContentConnectionString(Description.ConnectionInfo, Superuser);
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          foreach (var schemaDefinition in Description.Schemas)
          {
            cmd.CommandText = schemaDefinition.Load();
            cmd.ExecuteNonQuery();
          }

          if (csBuilder.ContainsKey(UserIdKey))
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
                                                 schema, csBuilder[UserIdKey]);
              }

              cmd.ExecuteNonQuery();
            }
          }
        }
      }
    }

    public void Destroy()
    {
      if (Description.ConnectionInfo == null) throw new ArgumentNullException("Description.ConnectionInfo");
      if (string.IsNullOrEmpty(Description.ConnectionInfo.ConnectionString)) throw new ArgumentException("Connection information is missing a connection string", "Description.ConnectionInfo.ConnectionString");
      if (Description.ConnectionInfo.ProviderFactory == null) throw new ArgumentException("Connection information is missing a provider factory", "Description.ConnectionInfo.ProviderFactory");
      

      var csBuilder = new DbConnectionStringBuilder {ConnectionString = Description.ConnectionInfo.ConnectionString};

      using (var conn = ConnectionInfo.ProviderFactory.CreateConnection())
      {
        conn.ConnectionString = CreateDatabaseConnectionString(Description.ConnectionInfo, Superuser);
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandText = string.Format("DROP DATABASE IF EXISTS \"{0}\"", csBuilder[DatabaseKey]);
          cmd.ExecuteNonQuery();

          if(csBuilder.ContainsKey(UserIdKey) && !((string)csBuilder[UserIdKey]).Equals(Superuser.UserId, StringComparison.InvariantCultureIgnoreCase))
          {
            // Delete the role if it is not in use by any databases
            cmd.CommandText = string.Format("SELECT COUNT(*) FROM pg_catalog.pg_shdepend sd " +
                                            "JOIN pg_catalog.pg_roles r ON r.oid = sd.refobjid " +
                                            "WHERE r.rolname='{0}' ", csBuilder[UserIdKey]);

            if (Convert.ToInt64(cmd.ExecuteScalar()) == 0)
            {
              cmd.CommandText = string.Format("DROP ROLE IF EXISTS \"{0}\"", csBuilder[UserIdKey]);
              cmd.ExecuteNonQuery();
            }
          }
        }
      }
    }

    public void Seed()
    {
      if (Description.ConnectionInfo == null) throw new ArgumentNullException("Description.ConnectionInfo");
      if (string.IsNullOrEmpty(Description.ConnectionInfo.ConnectionString)) throw new ArgumentException("Connection information is missing a connection string", "Description.ConnectionInfo.ConnectionString");
      if (Description.ConnectionInfo.ProviderFactory == null) throw new ArgumentException("Connection information is missing a provider factory", "Description.ConnectionInfo.ProviderFactory");
      

      using (var conn = ConnectionInfo.ProviderFactory.CreateConnection())
      {
        conn.ConnectionString = CreateContentConnectionString(Description.ConnectionInfo, Superuser);
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
    
    internal static string CreateDatabaseConnectionString(IDbConnectionInfo connectionInfo, PgSuperuser superuser)
    {
      if (connectionInfo == null) throw new ArgumentNullException("connectionInfo");
      if (superuser == null) throw new ArgumentNullException("superuser");
      if (string.IsNullOrEmpty(connectionInfo.ConnectionString)) throw new ArgumentException("Connection information is missing a connection string", "connectionInfo.ConnectionString");
         
      var csBuilder = new DbConnectionStringBuilder {ConnectionString = connectionInfo.ConnectionString};
      csBuilder[DatabaseKey] = superuser.Database;
      csBuilder[UserIdKey] = superuser.UserId;
      csBuilder[PasswordKey] = superuser.Password;
      return csBuilder.ConnectionString;
    }

    internal static string CreateContentConnectionString(IDbConnectionInfo connectionInfo, PgSuperuser superuser)
    {
      if (connectionInfo == null) throw new ArgumentNullException("connectionInfo");
      if (superuser == null) throw new ArgumentNullException("superuser");
      if (string.IsNullOrEmpty(connectionInfo.ConnectionString)) throw new ArgumentException("Connection information is missing a connection string", "connectionInfo.ConnectionString");
         
      var csBuilder = new DbConnectionStringBuilder {ConnectionString = connectionInfo.ConnectionString};
      csBuilder[UserIdKey] = superuser.UserId;
      csBuilder[PasswordKey] = superuser.Password;
      return csBuilder.ConnectionString;
    }

    public PgDbDescription Description { get; private set; }
    public PgSuperuser Superuser { get; private set; }

  }
}
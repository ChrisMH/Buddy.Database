using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Utility.Database.PostgreSql
{
  public class PgDbManager : IDbManager
  {
    public PgDbManager()
    {
      Superuser = new PgSuperuser();
    }
    
    public void Create()
    {
      Destroy();
      
      using (var conn = CreateDatabaseConnection())
      {
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {

          cmd.CommandText = string.Format("CREATE DATABASE \"{0}\"", Description.ConnectionInfo.DatabaseName);
          if (!string.IsNullOrEmpty(Description.TemplateName))
          {
            cmd.CommandText += string.Format(" TEMPLATE \"{0}\"", Description.TemplateName);
          }
          cmd.ExecuteNonQuery();

          if (!string.IsNullOrWhiteSpace(Description.ConnectionInfo.UserName) && !string.IsNullOrWhiteSpace(Description.ConnectionInfo.Password))
          {
            cmd.CommandText = string.Format("SELECT COUNT(*) FROM pg_catalog.pg_user WHERE usename='{0}'", Description.ConnectionInfo.UserName);
            if (Convert.ToInt64(cmd.ExecuteScalar()) == 0)
            {
              cmd.CommandText = string.Format("CREATE ROLE \"{0}\" LOGIN ENCRYPTED PASSWORD '{1}' NOINHERIT", Description.ConnectionInfo.UserName, Description.ConnectionInfo.Password);
              cmd.ExecuteNonQuery();
            }
          }
        }
      }

      using (var conn = CreateContentConnection())
      {
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          foreach (var schemaDefinition in Description.Schemas)
          {
            cmd.CommandText = schemaDefinition.Load();
            cmd.ExecuteNonQuery();
          }

          if (!string.IsNullOrWhiteSpace(Description.ConnectionInfo.UserName))
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
                                                 schema, Description.ConnectionInfo.UserName);
              }

              cmd.ExecuteNonQuery();
            }
          }
        }
      }
    }

    public void Destroy()
    {
      CheckPreconditions();
      
      using (var conn = CreateDatabaseConnection())
      {
        conn.Open();

        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandText = string.Format("DROP DATABASE IF EXISTS \"{0}\"", Description.ConnectionInfo.DatabaseName);
          cmd.ExecuteNonQuery();

          if(!string.IsNullOrWhiteSpace(Description.ConnectionInfo.UserName) && !Description.ConnectionInfo.UserName.Equals(Superuser.UserName, StringComparison.InvariantCultureIgnoreCase))
          {
            // Delete the role if it is not in use by any databases
            cmd.CommandText = string.Format("SELECT COUNT(*) FROM pg_catalog.pg_shdepend sd " +
                                            "JOIN pg_catalog.pg_roles r ON r.oid = sd.refobjid " +
                                            "WHERE r.rolname='{0}' ", Description.ConnectionInfo.UserName);

            if (Convert.ToInt64(cmd.ExecuteScalar()) == 0)
            {
              cmd.CommandText = string.Format("DROP ROLE IF EXISTS \"{0}\"", Description.ConnectionInfo.UserName);
              cmd.ExecuteNonQuery();
            }
          }
        }
      }
    }

    public void Seed()
    {
      CheckPreconditions();

      using (var conn = CreateContentConnection())
      {
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
      get { return Description == null ? null : Description.ConnectionInfo; }
    }
    
    public PgDbDescription Description { get; set; }
    public PgSuperuser Superuser { get; set; }


    internal DbConnection CreateDatabaseConnection()
    {
      CheckPreconditions();

      var csBuilder = new DbConnectionStringBuilder {ConnectionString = Description.ConnectionInfo.ConnectionString};
      csBuilder[PgDbConnectionInfo.DatabaseNameKey] = Superuser.DatabaseName;
      csBuilder[PgDbConnectionInfo.UserNameKey] = Superuser.UserName;
      csBuilder[PgDbConnectionInfo.PasswordKey] = Superuser.Password;

      var conn = ((IDbProviderInfo)Description.ConnectionInfo).ProviderFactory.CreateConnection();
      conn.ConnectionString = csBuilder.ConnectionString;

      return conn;
    }

    internal DbConnection CreateContentConnection()
    {
      CheckPreconditions();

      var csBuilder = new DbConnectionStringBuilder {ConnectionString = Description.ConnectionInfo.ConnectionString};
      csBuilder[PgDbConnectionInfo.UserNameKey] = Superuser.UserName;
      csBuilder[PgDbConnectionInfo.PasswordKey] = Superuser.Password;
      
      var conn = ((IDbProviderInfo)Description.ConnectionInfo).ProviderFactory.CreateConnection();
      conn.ConnectionString = csBuilder.ConnectionString;

      return conn;
    }
    
    protected void CheckPreconditions()
    {
      if (Superuser == null) throw new ArgumentNullException("Superuser");
      if (Description == null) throw new ArgumentNullException("Description");
      if (Description.ConnectionInfo == null) throw new ArgumentNullException("Description.ConnectionInfo");
      if (string.IsNullOrEmpty(Description.ConnectionInfo.ConnectionString)) throw new ArgumentException("Connection information is missing a connection string", "Description.ConnectionInfo.ConnectionString");
      if (((IDbProviderInfo) Description.ConnectionInfo).ProviderFactory == null) throw new ArgumentException("Connection information is missing a provider factory", "Description.ConnectionInfo.ProviderFactory");
    }
  }
}
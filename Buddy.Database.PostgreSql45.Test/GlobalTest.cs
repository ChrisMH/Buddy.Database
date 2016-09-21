using System;
using Npgsql;
using NUnit.Framework;
using Buddy.Database;

namespace Buddy.Database.PostgreSql.Test
{
    [SetUpFixture]
    public class GlobalTest
    {
        public static PgDbManager Manager1 { get; private set; }
        public static PgDbManager Manager2 { get; private set; }
        public static PgSuperuser Superuser = new PgSuperuser();

        [OneTimeSetUp]
        public void SetUp()
        {
            Manager1 = new PgDbManager
            {
                Description = new PgDbDescription
                {
                    ConnectionInfo = new DbConnectionInfo {ConnectionStringName = "Test1"},
                    Superuser = Superuser
                }
            };

            Manager2 = new PgDbManager
            {
                Description = new PgDbDescription
                {
                    ConnectionInfo = new DbConnectionInfo {ConnectionStringName = "Test2"},
                    Superuser = Superuser
                }
            };
        }

        [OneTimeTearDown]
        public void TearDown()
        {
        }

        public static void DropTestDatabaseAndRole()
        {
            NpgsqlConnection.ClearAllPools();
            using (var conn = Manager1.CreateDatabaseConnection())
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("DROP DATABASE IF EXISTS \"{0}\"", Manager1.Description.ConnectionInfo.DatabaseName);
                    cmd.ExecuteNonQuery();
                }
            }

            using (var conn = Manager2.CreateDatabaseConnection())
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("DROP DATABASE IF EXISTS \"{0}\"", Manager2.Description.ConnectionInfo.DatabaseName);
                    cmd.ExecuteNonQuery();
                }
            }

            using (var conn = Manager1.CreateDatabaseConnection())
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("DROP ROLE IF EXISTS \"{0}\"", Manager2.Description.ConnectionInfo.UserName);
                    cmd.ExecuteNonQuery();
                }
            }

            using (var conn = Manager2.CreateDatabaseConnection())
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("DROP ROLE IF EXISTS \"{0}\"", Manager2.Description.ConnectionInfo.UserName);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
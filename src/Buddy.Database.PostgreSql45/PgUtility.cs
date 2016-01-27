using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Buddy.Database;
using Npgsql;

namespace Buddy.Database.PostgreSql
{
    public static class PgUtility
    {
        public static string CreateInsert(List<PgParameter> parameterMap, string insertSqlFormat, List<object> fieldValues)
        {
            var fields = "";
            var values = "";
            var fieldIndex = fieldValues.Count;
            foreach (var parameterEntry in parameterMap)
            {
                if (!parameterEntry.IncludeCheck(parameterEntry.Values))
                    continue;

                fields = string.Concat(fields, fields.Length > 0 ? "," : "", parameterEntry.FieldName);
                values = string.Concat(values, values.Length > 0 ? "," : "", parameterEntry.ValueGenerator(ref fieldIndex, fieldValues, parameterEntry.Values));
            }

            if (string.IsNullOrWhiteSpace(fields) || string.IsNullOrWhiteSpace(values))
                return null;
            return string.Format(insertSqlFormat, fields, values);
        }

        public static string CreateUpdate(List<PgParameter> parameterMap, string updateSqlFormat, List<object> fieldValues)
        {
            var sets = "";
            var fieldIndex = fieldValues.Count;
            foreach (var parameterEntry in parameterMap)
            {
                if (!parameterEntry.IncludeCheck(parameterEntry.Values))
                    continue;

                sets = string.Concat(
                    sets,
                    sets.Length > 0 ? "," : "",
                    parameterEntry.FieldName,
                    "=",
                    parameterEntry.ValueGenerator(ref fieldIndex, fieldValues, parameterEntry.Values)
                    );
            }

            if (string.IsNullOrWhiteSpace(sets))
                return null;
            return string.Format(updateSqlFormat, sets);
        }

        public static bool NonWhitespaceStringIncludeCheck(object[] values)
        {
            return values.Count(v => string.IsNullOrWhiteSpace((string)v)) == 0;
        }

        public static bool NotNullIncludeCheck(object[] values)
        {
            return values.Count(v => v == null) == 0;
        }

        public static string SimpleValueGenerator(ref int fieldIndex, List<object> fieldValues, params object[] values)
        {
            if (values == null || values.Length != 1)
                throw new ArgumentException("values not supplied", "values");

            fieldValues.Add(values[0]);
            return string.Format(":p{0}", fieldIndex++);
        }

        public static string GeographicPointValueGenerator(ref int fieldIndex, List<object> fieldValues, params object[] values)
        {
            if (values == null || values.Length != 3)
                throw new ArgumentException("values not supplied", "values");

            fieldValues.AddRange(values);
            fieldIndex += 3;
            return string.Format("ST_SetSRID(ST_MakePoint(:p{0},:p{1},:p{2}), 4326)", fieldIndex - 3, fieldIndex - 2, fieldIndex - 1);
        }

        public static string CreateDbLinkConnection(IDbConnectionInfo connectionInfo)
        {
            var builder = new NpgsqlConnectionStringBuilder(connectionInfo.ConnectionString);
            var dbLink = new StringBuilder();
            
            if (builder.ContainsKey("host"))
            {
                if (dbLink.Length > 0) dbLink.Append(" ");
                dbLink.Append("host=").Append(builder["host"]);
            }

            if (builder.ContainsKey("port"))
            {
                if (dbLink.Length > 0) dbLink.Append(" ");
                dbLink.Append("port=").Append(builder["port"]);
            }

            if (builder.ContainsKey("database"))
            {
                if (dbLink.Length > 0) dbLink.Append(" ");
                dbLink.Append("dbname=").Append(builder["database"]);
            }

            if (builder.ContainsKey("username"))
            {
                if (dbLink.Length > 0) dbLink.Append(" ");
                dbLink.Append("user=").Append(builder["username"]);
            }

            if (builder.ContainsKey("password"))
            {
                if (dbLink.Length > 0) dbLink.Append(" ");
                dbLink.Append("password=").Append(builder["password"]);
            }
            return dbLink.ToString();

        }
    }
}
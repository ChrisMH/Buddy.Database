using System;

namespace Utility.Database.PostgreSql
{
  public sealed class PgDbConnectionInfo : GenericDbConnectionInfo
  {
    internal const string ServerAddressKey = "host";
    internal const string ServerPortKey = "port";
    internal const string DatabaseNameKey = "database";
    internal const string UserNameKey = "user id";
    internal const string PasswordKey = "password";

    public override string ServerAddress
    {
      get { return connectionString.ContainsKey(ServerAddressKey) ? (string)connectionString[ServerAddressKey] : null; }
    }

    public override int? ServerPort
    {
      get { return connectionString.ContainsKey(ServerPortKey) ? (int?)Convert.ToInt32(connectionString[ServerPortKey]) : null; }
    }

    public override string DatabaseName
    {
      get { return connectionString.ContainsKey(DatabaseNameKey) ? (string)connectionString[DatabaseNameKey] : null; }
    }

    public override string UserName
    {
      get { return connectionString.ContainsKey(UserNameKey) ? (string)connectionString[UserNameKey] : null; }
    }

    public override string Password
    {
      get { return connectionString.ContainsKey(PasswordKey) ? (string)connectionString[PasswordKey] : null; }
    }

    public override IDbConnectionInfo Copy()
    {
      var copy = new PgDbConnectionInfo();
      InternalCopy(copy);
      return copy;
    }
  }
}
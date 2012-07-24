namespace Utility.Database.PostgreSql
{
  public class PgSuperuser : IDbSuperuser
  {
    public PgSuperuser()
    {
      DatabaseName = "postgres";
      UserName = "postgres";
      Password = "postgres";
    }

    public string DatabaseName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
  }
}

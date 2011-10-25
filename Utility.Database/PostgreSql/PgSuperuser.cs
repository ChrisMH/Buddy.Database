namespace Utility.Database.PostgreSql
{
  public class PgSuperuser
  {
    public PgSuperuser()
    {
      Database = "postgres";
      UserId = "postgres";
      Password = "postgres";
    }

    public string Database { get; set; }
    public string UserId { get; set; }
    public string Password { get; set; }
  }
}

namespace Utility.Database
{
  public interface IDbConnectionInfo
  {
    /// <summary>
    /// Loads connection information using the connection string name
    /// as typically set in a web.config or app.config file
    /// </summary>
    string ConnectionStringName { get; set; }

    string ConnectionString { get; set; }

    /// <summary>
    /// Used to retrieve individual pieces of the connection string
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    object this[string key] { get; }

    IDbConnectionInfo Copy();
  }
}
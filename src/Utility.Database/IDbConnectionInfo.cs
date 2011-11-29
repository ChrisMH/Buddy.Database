namespace Utility.Database
{
  public interface IDbConnectionInfo
  {
    string Name { get; set; }
    string ConnectionString { get; set; }

    /// <summary>
    /// Used to retrieve individual pieces of the connection string
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    object this[string key] { get; }
  }
}
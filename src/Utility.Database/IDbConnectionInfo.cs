namespace Utility.Database
{
  public interface IDbConnectionInfo
  {
    /// <summary>
    /// Get or set the connection string name.
    /// 
    /// When set, loads connection information ConfigurationManager.ConnectionStrings
    /// as typically specified in a web.config or app.config file
    /// </summary>
    string ConnectionStringName { get; set; }

    /// <summary>
    /// Get or set the full connection string
    /// </summary>
    string ConnectionString { get; set; }

    /// <summary>
    /// Test if connection string piece is available
    /// </summary>
    /// <param name="key">Connection string piece</param>
    /// <returns>True if the piece is availabe, False otherwise</returns>
    bool ContainsKey(string key);

    /// <summary>
    /// Get individual pieces of the connection string
    /// </summary>
    /// <param name="key">Connection string piece</param>
    /// <returns>The key's value, or null if it does not have the requested value</returns>
    object this[string key] { get; }



    /// <summary>
    /// Makes a copy of this instance
    /// </summary>
    /// <returns>A copy of this instance</returns>
    IDbConnectionInfo Copy();
  }
}
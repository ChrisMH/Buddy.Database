namespace Utility.Database.Management
{
  public interface IDbManager
  {
    void Create();
    void Destroy();
    void Seed();

    IDbConnection Connection { get; }
  }
}
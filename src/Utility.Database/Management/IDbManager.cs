namespace Utility.Database.Management
{
  public interface IDbManager
  {
    void Create();
    void Destroy();
    void Seed();

    IDbConnectionInfo ConnectionInfo { get; }
  }
}
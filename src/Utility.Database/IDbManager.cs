namespace Utility.Database
{
  public interface IDbManager
  {
    void Create();
    void Destroy();
    void Seed();

    IDbConnectionInfo ConnectionInfo { get; }
  }
}
namespace Utility.Database
{
  public interface IDbManager
  {
    void Create();
    void Destroy();
    void Seed();

    IDbDescription Description { get; set; }
  }
}
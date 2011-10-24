using MicrOrm;

namespace Utility.Database
{
  public interface IDbCreator
  {
    void Create();
    void Destroy();
    void Seed();

    IMoConnectionProvider Provider { get; }
  }
}
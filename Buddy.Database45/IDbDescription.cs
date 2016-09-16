using System.Collections.Generic;

namespace Buddy.Database
{
  public interface IDbDescription
  {
    IDbConnectionInfo ConnectionInfo { get; set; }
    IDbSuperuser Superuser { get; set; }
    List<DbScript> Schemas { get; }
    List<DbScript> Seeds { get; }
  }
}
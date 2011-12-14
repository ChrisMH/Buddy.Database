using System.Collections.Generic;

namespace Utility.Database
{
  public interface IDbDescription
  {
    IDbConnectionInfo ConnectionInfo { get; set; }
    List<DbScript> Schemas { get; }
    List<DbScript> Seeds { get; }
  }
}
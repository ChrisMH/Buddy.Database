using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.Database.Mock
{
  public interface IDbMockTypeInfo
  {
    Type MockDatabaseType { get; }
  }
}

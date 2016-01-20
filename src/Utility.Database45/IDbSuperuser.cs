using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.Database
{
  public interface IDbSuperuser
  {
    string UserName { get; set; }
    string Password { get; set; }
  }
}

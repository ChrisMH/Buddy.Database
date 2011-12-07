using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.Database.Mock
{
  public class MockDbManager : IDbManager
  {
    public void Create()
    {
      Destroy(); // Checks preconditions

      MockDatabaseProvider.Create(Description.ConnectionInfo);

      foreach (var schema in Description.Schemas)
      {
        if (schema.ScriptType != ScriptType.Runnable) throw new ArgumentException("Mock database only support the 'Runnable' ScriptType", "ScriptType");
        schema.Run(Description.ConnectionInfo);
      }
    }

    public void Destroy()
    {
      CheckPreconditions();

      MockDatabaseProvider.Destroy(Description.ConnectionInfo);
    }

    public void Seed()
    {
      CheckPreconditions();

      foreach (var seed in Description.Seeds)
      {
        if (seed.ScriptType != ScriptType.Runnable) throw new ArgumentException("Mock database only support the 'Runnable' ScriptType", "ScriptType");
        seed.Run(Description.ConnectionInfo);
      }
    }

    public IDbDescription Description { get; set; }

    protected void CheckPreconditions()
    {
      if (Description == null) throw new ArgumentNullException("Description", "Description is not set");
      if (Description.ConnectionInfo == null) throw new ArgumentNullException("Description.ConnectionInfo", "ConnectionInfo is not set");
    }


  }
}

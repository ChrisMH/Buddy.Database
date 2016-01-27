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
      VerifyProperties();

      MockDatabaseProvider.Destroy(Description.ConnectionInfo);
    }

    public void Seed()
    {
      VerifyProperties();

      foreach (var seed in Description.Seeds)
      {
        if (seed.ScriptType != ScriptType.Runnable) throw new ArgumentException("Mock database only support the 'Runnable' ScriptType", "ScriptType");
        seed.Run(Description.ConnectionInfo);
      }
    }

    public IDbDescription Description { get; set; }

    protected void VerifyProperties()
    {
      if (Description == null) throw new ArgumentException("Description is null", "Description");
      if (Description.ConnectionInfo == null) throw new ArgumentException("ConnectionInfo is null", "Description.ConnectionInfo");
      if(string.IsNullOrWhiteSpace(Description.ConnectionInfo.ConnectionString)) throw new ArgumentException("Description.ConnectionInfo.ConnectionString not provided", "Description.ConnectionInfo.ConnectionString");

      if(Description.ConnectionInfo.GetType() != typeof(MockDbConnectionInfo))
      {
        Description.ConnectionInfo = new MockDbConnectionInfo {ConnectionString = Description.ConnectionInfo.ConnectionString};
      }
    }
  }
}

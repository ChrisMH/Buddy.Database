using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.Database.Mock.Test
{
  public class MockSeed2
  {
    public void Run(IDbConnectionInfo connectionInfo)
    {
      var db = MockDatabaseProvider.Open(connectionInfo) as TestMockDatabase;
      if (db == null) throw new ArgumentException("Connection information is not a mock database", "connectionInfo");

      db.Table.Add(new TestMockDatabase.Row { Id = 3, RowName = "Three" });
    }
  }
}

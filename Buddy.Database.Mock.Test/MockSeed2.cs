using System;

namespace Utility.Database.Mock.Test
{
  public class MockSeed2
  {
    public void Run(IDbConnectionInfo connectionInfo)
    {
      var db = MockDatabaseProvider.Open(connectionInfo) as TestMockDatabase;
      if (db == null) throw new ArgumentException("Could not open mock database from supplied connectionInfo", "connectionInfo");

      db.Table.Add(new TestMockDatabase.Row {Id = 3, RowName = "Three"});
    }
  }
}
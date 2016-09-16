using System;

namespace Utility.Database.Mock.Test
{
  public class MockSeed1
  {
    public void Run(IDbConnectionInfo connectionInfo)
    {
      var db = MockDatabaseProvider.Open(connectionInfo) as TestMockDatabase;
      if (db == null) throw new ArgumentException("Could not open mock database from supplied connectionInfo", "connectionInfo");

      db.Table.Clear();

      db.Table.Add(new TestMockDatabase.Row {Id = 1, RowName = "One"});
      db.Table.Add(new TestMockDatabase.Row {Id = 2, RowName = "Two"});
    }
  }
}
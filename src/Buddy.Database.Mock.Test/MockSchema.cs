using System;

namespace Utility.Database.Mock.Test
{
  public class MockSchema
  {
    public void Run(IDbConnectionInfo connectionInfo)
    {
      var db = MockDatabaseProvider.Open(connectionInfo) as TestMockDatabase;
      if (db == null) throw new ArgumentException("Could not open mock database from supplied connectionInfo", "connectionInfo");

      // Schema already defined in TestMockDatabase

      // TODO: Is there a point to this for mock databases?
    }
  }
}
using System.Collections.Generic;

namespace Utility.Database.Mock.Test
{
  public class TestMockDatabase : IMockDatabase
  {
    public TestMockDatabase()
    {
      Table = new List<Row>();
    }

    public List<Row> Table { get; set; }

    public class Row
    {
      public int Id { get; set; }
      public string RowName { get; set; }
    }
  }

  public class TestMockDatabaseInvalidConstructor : IMockDatabase
  {
    public TestMockDatabaseInvalidConstructor(string parameter)
    {
      
    }
  }

  public class TestMockDatabaseInvalidBaseInterface
  {
    public TestMockDatabaseInvalidBaseInterface()
    {

    }
  }
}
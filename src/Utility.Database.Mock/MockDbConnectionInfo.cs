using System;
using System.Collections.Generic;

namespace Utility.Database.Mock
{
  public sealed class MockDbConnectionInfo : IDbConnectionInfo, IDbMockTypeInfo
  {
    public string ConnectionStringName { get; set; }
    public Type MockDatabaseType { get; set; }

    public string ConnectionString
    {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public string ServerAddress
    {
      get { throw new NotImplementedException(); }
    }

    public int? ServerPort
    {
      get { throw new NotImplementedException(); }
    }

    public string DatabaseName
    {
      get { throw new NotImplementedException(); }
    }

    public string UserName
    {
      get { throw new NotImplementedException(); }
    }

    public string Password
    {
      get { throw new NotImplementedException(); }
    }

    public IDbConnectionInfo Copy()
    {
      return new MockDbConnectionInfo {ConnectionStringName = ConnectionStringName, MockDatabaseType = MockDatabaseType};
    }
  }
}
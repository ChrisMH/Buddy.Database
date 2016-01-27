using System;

namespace Buddy.Database.Test
{
  public class Runnable
  {
    public void Run(IDbConnectionInfo connectionInfo)
    {
      throw new Exception("Method was called");
    }
  }
  public class RunnableWithInvalidMethodName
  {
    public void InvalidMethodName(IDbConnectionInfo connectionInfo)
    {
    }
  }
  public class RunnableWithInvalidReturnType
  {
    public string Run(IDbConnectionInfo connectionInfo)
    {
      return null;
    }
  }

  public class RunnableWithInvalidMethodSignature
  {
    public void Run(string connectionInfo)
    {
    }
  }
}

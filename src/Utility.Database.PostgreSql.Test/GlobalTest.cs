using System;
using MicrOrm.Core;
using NUnit.Framework;
using Utility.Logging;
using Utility.Logging.NLog;

namespace Utility.Database.PostgreSql.Test
{
  [SetUpFixture]
  public class GlobalTest
  {
    public static ILogger Logger { get; private set; }
    
    [SetUp]
    public void SetUp()
    {
      Logger = new NLogLoggerFactory().GetLogger("Utility.Database.PostgreSql.Test");

      try
      {

        MoLogger.Logger = Logger;
        MoLogger.Enabled = true;
      }
      catch (Exception e)
      {
        if(Logger != null) Logger.Fatal(e, "SetUp : {0} : {1}", e.GetType(), e.Message);
        throw;
      }
    }

    [TearDown]
    public void TearDown()
    {
      try
      {
      }
      catch (Exception e)
      {
        if (Logger != null) Logger.Fatal(e, "TearDown : {0} : {1}", e.GetType(), e.Message);
        throw;
      }
    }
  }
}
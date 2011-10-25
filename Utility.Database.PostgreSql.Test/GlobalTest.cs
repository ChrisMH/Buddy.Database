using System;
using MicrOrm.Core;
using NUnit.Framework;
using Utility.Logging;
using Utility.Logging.NLog;

namespace Utility.Database.PostgreSql.Test
{
  [SetUpFixture]
  public static class GlobalTest
  {
    public static ILogger Logger { get; private set; }

    static GlobalTest()
    {
      Logger = new NLogLoggerFactory().GetLogger("Utility.Database.PostgreSql.Test");

      try
      {
        MoLogger.Logger = Logger;
        MoLogger.Enabled = true;
      }
      catch (Exception e)
      {
        Logger.Fatal(e, "GlobalTest : {0} : {1}", e.GetType(), e.Message);
        throw;
      }
    }

    [SetUp]
    public static void SetUp()
    {
      try
      {
      }
      catch (Exception e)
      {
        Logger.Fatal(e, "SetUp : {0} : {1}", e.GetType(), e.Message);
        throw;
      }
    }

    [TearDown]
    public static void TearDown()
    {
      try
      {
      }
      catch (Exception e)
      {
        Logger.Fatal(e, "TearDown : {0} : {1}", e.GetType(), e.Message);
        throw;
      }
    }
  }
}
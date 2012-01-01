using System;
using NUnit.Framework;
using Utility.Logging;
using Utility.Logging.NLog;

namespace Utility.Database.Mock.Test
{
  [SetUpFixture]
  public class GlobalTest
  {
    public static IDbManager DbManager1 { get; private set; }
    public static ILogger Logger { get; private set; }

    [SetUp]
    public void SetUp()
    {
      try
      {
        Logger = new NLogLoggerFactory().GetCurrentInstanceLogger();

        DbManager1 = new MockDbManager
                       {
                         Description = new DbDescription
                                         {
                                           XmlRoot = DbDescriptions.Valid,
                                           ConnectionInfo = new MockDbConnectionInfo
                                                              {
                                                                ConnectionStringName = "Test1"
                                                              }
                                         }
                       };
      }
      catch (Exception e)
      {
        if (Logger != null) Logger.Fatal(e, "SetUp : {0} : {1}", e.GetType(), e.Message);
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
using System;
using NUnit.Framework;

namespace Utility.Database.Mock.Test
{
  [SetUpFixture]
  public class GlobalTest
  {
    public static IDbManager DbManager1 { get; private set; }
    public static NLog.Logger Logger { get; private set; }

    [SetUp]
    public void SetUp()
    {
      try
      {
        Logger = NLog.LogManager.GetCurrentClassLogger();

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
        if (Logger != null) Logger.FatalException(string.Format("SetUp : {0} : {1}", e.GetType(), e.Message), e);
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
        if (Logger != null) Logger.FatalException(string.Format("TearDown : {0} : {1}", e.GetType(), e.Message), e);
        throw;
      }
    }
  }
}
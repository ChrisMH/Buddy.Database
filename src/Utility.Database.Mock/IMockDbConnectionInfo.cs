using System;

namespace Utility.Database.Mock
{
  internal interface IMockDbConnectionInfo
  {
    Type DatabaseType { get; }
  }
}
using System;
using System.Data.Common;
using System.Xml.Linq;

namespace Utility.Database.PostgreSql
{
  public class PgDbDescription : DbDescription
  {
    public PgDbDescription()
    {
      AllowPooling = false;
    }

    public PgDbDescription(XElement root, string baseDirectory = null)
      : base(root, baseDirectory)
    {
      if (root.Element("TemplateName") != null)
      {
        TemplateName = root.Element("TemplateName").Value;
        if (string.IsNullOrEmpty(TemplateName)) throw new ArgumentException("TemplateName element is empty", "TemplateName");
      }
    }

    public String TemplateName { get; set; }
    public bool AllowPooling { get; set; }

    public override IDbConnectionInfo ConnectionInfo
    {
      get { return AdjustConnectionInfoPooling(base.ConnectionInfo, AllowPooling); }
    }

    protected static IDbConnectionInfo AdjustConnectionInfoPooling(IDbConnectionInfo connectionInfo, bool allowPooling)
    {
      if (connectionInfo != null && !allowPooling)
      {
        if(!string.IsNullOrEmpty(connectionInfo.ConnectionString))
        {
          var csBuilder = new DbConnectionStringBuilder {ConnectionString = connectionInfo.ConnectionString};
          csBuilder["pooling"] = "false";
          connectionInfo.ConnectionString = csBuilder.ConnectionString;
        }
      }
      return connectionInfo;
    }
  }
}
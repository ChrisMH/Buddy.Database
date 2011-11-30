using System;
using System.Data.Common;
using System.Xml.Linq;

namespace Utility.Database.PostgreSql
{
  public class PgDbDescription : DbDescription<GenericDbConnectionInfo>
  {
    public PgDbDescription()
    {
      AllowPooling = false;
    }
    
    public String TemplateName { get; set; }
    public bool AllowPooling { get; set; }

    public override IDbConnectionInfo ConnectionInfo
    {
      get { return AdjustConnectionInfoPooling(base.ConnectionInfo, AllowPooling); }
    }

    public override string XmlRoot
    {
      set
      {
        base.XmlRoot = value;

        var root = XElement.Parse(value);
        if (root.Element("TemplateName") != null)
        {
          TemplateName = root.Element("TemplateName").Value;
          if (string.IsNullOrEmpty(TemplateName)) throw new ArgumentException("TemplateName element is empty", "TemplateName");
        }
      }
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
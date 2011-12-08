using System;
using System.Data.Common;
using System.Xml.Linq;

namespace Utility.Database.PostgreSql
{
  public class PgDbDescription : DbDescription<PgDbConnectionInfo>
  {
    public String TemplateName { get; set; }

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
  }
}
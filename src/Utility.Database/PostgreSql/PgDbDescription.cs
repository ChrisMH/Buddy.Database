using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Utility.Database.PostgreSql
{
  public class PgDbDescription : DbDescription
  {
    public PgDbDescription()
    {
    }

    public PgDbDescription(XElement root, string baseDirectory = null)
      : base(root, baseDirectory)
    {
      if(root.Element("TemplateName") != null)
      {
        TemplateName = root.Element("TemplateName").Value;
        if(string.IsNullOrEmpty(TemplateName)) throw new ArgumentException("TemplateName element is empty", "TemplateName");
      }
      
    }

    public String TemplateName { get; set; }
  }
}

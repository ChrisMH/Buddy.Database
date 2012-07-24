using System;
using System.Xml.Linq;

namespace Utility.Database.PostgreSql
{
  public class PgDbDescription : DbDescription
  {
    public PgDbDescription()
    {
      Superuser = new PgSuperuser();
    }

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

        var superuserRoot = root.Element("Superuser");
        if (superuserRoot != null)
        {
          if(superuserRoot.Element("UserName") != null)
          {
            Superuser.UserName = superuserRoot.Element("UserName").Value;
            if (string.IsNullOrEmpty(Superuser.UserName)) throw new ArgumentException("Superuser.UserName element is empty", "Superuser.UserName");
          }

          if (superuserRoot.Element("Password") != null)
          {
            Superuser.Password = superuserRoot.Element("Password").Value;
            if (string.IsNullOrEmpty(Superuser.Password)) throw new ArgumentException("Superuser.Password element is empty", "Superuser.Password");
          }
        }
      }
    }
  }
}
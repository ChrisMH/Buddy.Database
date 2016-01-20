using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Utility.Database
{
  public class DbDescription : IDbDescription 
  {
    public DbDescription()
    {
      Schemas = new List<DbScript>();
      Seeds = new List<DbScript>();
    }

    public virtual IDbConnectionInfo ConnectionInfo
    {
      get { return connectionInfo == null ? null : connectionInfo.Copy(); }
      set { connectionInfo = value == null ? null : value.Copy(); }
    }

    public List<DbScript> Schemas { get; set; }
    public List<DbScript> Seeds { get; set; }
    public IDbSuperuser Superuser { get; set; }

    public string BaseDirectory
    {
      get { return string.IsNullOrWhiteSpace(baseDirectory) ? AppDomain.CurrentDomain.BaseDirectory : baseDirectory; }
      set { baseDirectory = value; }
    }

    /// <summary>
    /// XML description of the database 
    /// </summary>
    public virtual string XmlRoot
    {
      set
      {
        try
        {
          var root = XElement.Parse(value);

          if (root.Element("Connection") != null)
          {
            var connectionElement = root.Element("Connection");

            if (connectionElement.Element("ConnectionStringName") != null)
            {
              connectionInfo = new DbConnectionInfo {ConnectionStringName = connectionElement.Element("ConnectionStringName").Value};
            }
            else if (connectionElement.Element("ConnectionString") != null)
            {
              connectionInfo = new DbConnectionInfo {ConnectionString = connectionElement.Element("ConnectionString").Value};
              
              if (connectionElement.Element("Provider") != null)
              {
                connectionInfo.Provider = connectionElement.Element("Provider").Value;
              }
            }
            else
            {
              throw new ArgumentException("Connection element must contain EITHER ConnectionStringName OR ConnectionString (and optional Provider)", "Connection");
            }
          }

          if (root.Elements("Schema").Any())
          {
            Schemas = root.Elements("Schema")
              .Select(e => new DbScript
                           {
                             XmlRoot = e.ToString(),
                             GetBaseDirectory = () => BaseDirectory
                           }
              )
              .ToList();
          }
          if (root.Elements("Seed").Any())
          {
            Seeds = root.Elements("Seed")
              .Select(e => new DbScript
                           {
                             XmlRoot = e.ToString(),
                             GetBaseDirectory = () => BaseDirectory
                           }
              )
              .ToList();
          }
        }
        catch (ArgumentException e)
        {
          throw new ArgumentException(e.Message, "XmlRoot", e);
        }
      }
    }


    private IDbConnectionInfo connectionInfo;
    private string baseDirectory;

    
    /// <summary>
    /// Create from a DbDescription node containing a type
    /// 
    /// &lt;DbDescription type="assembly.class, assembly"&gt;
    ///   ...
    /// &lt;/DbDescription&gt;
    /// 
    /// If the DbDescription element does not have an explicit 'type' attribute, Utility.Database.DbDescription is used
    /// </summary>
    /// <param name="xmlRoot"></param>
    /// <returns></returns>
    public static IDbDescription Create(string xmlRoot, string baseDirectory = null)
    {
      if (string.IsNullOrWhiteSpace(xmlRoot)) throw new ArgumentException("xmlRoot is invalid", "xmlRoot");

      var root = XElement.Parse(xmlRoot);

      DbDescription dbDescription;

      var typeAttribute = root.Attribute("type");
      if (typeAttribute == null)
      {
        dbDescription = new DbDescription();
      }
      else
      {
        try
        {
          dbDescription = new ReflectionType(typeAttribute.Value).CreateObject<DbDescription>();
        }
        catch (Exception e)
        {
          throw new ArgumentException(string.Format("Could not create database description type '{0}'", typeAttribute.Value), "xmlRoot", e);
        }
      }

      dbDescription.XmlRoot = xmlRoot;
      if(baseDirectory != null)
      {
        dbDescription.baseDirectory = baseDirectory;
      }

      return dbDescription;
    }
  }
}
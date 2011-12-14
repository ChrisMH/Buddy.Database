﻿using System;
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

    public string BaseDirectory
    {
      get { return string.IsNullOrWhiteSpace(baseDirectory) ? AppDomain.CurrentDomain.BaseDirectory : baseDirectory; }
      set { baseDirectory = value; }
    }

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
  }
}
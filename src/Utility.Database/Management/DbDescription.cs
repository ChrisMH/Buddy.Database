using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Utility.Database.Management
{
  public class DbDescription
  {
    public DbDescription()
    {
      Schemas = new List<DbScript>();
      Seeds = new List<DbScript>();
    }

    public DbDescription(XElement root, string baseDirectory = null)
      : this()
    {
      if (root.Element("Connection") != null)
      {
        try
        {
          var connectionElement = root.Element("Connection");

          if (connectionElement.Element("ConnectionStringName") != null)
          {
            ConnectionInfo = new DbConnectionInfo(connectionElement.Element("ConnectionStringName").Value);
          }
          else if (connectionElement.Element("ConnectionString") != null && connectionElement.Element("Provider") != null)
          {
            ConnectionInfo = new DbConnectionInfo
                         {
                           ConnectionString = connectionElement.Element("ConnectionString").Value,
                           Provider = connectionElement.Element("Provider").Value
                         };
          }
          else
          {
            throw new ArgumentException("Connection element must contain EITHER ConnectionStringName OR ConnectionString AND Provider", "Connection");
          }
        }
        catch(ArgumentException e)
        {
          throw new ArgumentException(e.Message, "Connection", e);
        }
      }
      if(root.Elements("Schema").Any())
      {
        Schemas = root.Elements("Schema").Select(e => new DbScript(e, baseDirectory)).ToList();
      }
      if(root.Elements("Seed").Any())
      {
        Seeds = root.Elements("Seed").Select(e => new DbScript(e, baseDirectory)).ToList();
      }
    }

    public virtual IDbConnectionInfo ConnectionInfo
    {
      get { return connectionInfo == null ? null : new DbConnectionInfo(connectionInfo); }
      set { connectionInfo = value == null ? null : new DbConnectionInfo(value); }
    }

    public List<DbScript> Schemas { get; set; }
    public List<DbScript> Seeds { get; set; }

    private DbConnectionInfo connectionInfo;
  }
}
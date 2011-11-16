using System;
using System.Collections.Generic;
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
            Connection = new DbConnection(connectionElement.Element("ConnectionStringName").Value);
          }
          else if (connectionElement.Element("ConnectionString") != null && connectionElement.Element("ProviderName") != null)
          {
            Connection = new DbConnection
                         {
                           ConnectionString = connectionElement.Element("ConnectionString").Value,
                           ProviderName = connectionElement.Element("ProviderName").Value
                         };
          }
          else
          {
            throw new ArgumentException("Connection element must contain EITHER ConnectionStringName OR ConnectionString AND ProviderName", "Connection");
          }
        }
        catch(ArgumentException e)
        {
          throw new ArgumentException(e.Message, "Connection", e);
        }
      }
      root.Elements("Schema").ForEach(element => Schemas.Add(new DbScript(element, baseDirectory)));
      root.Elements("Seed").ForEach(element => Seeds.Add(new DbScript(element, baseDirectory)));
    }

    public IDbConnection Connection { get; set; }
    public List<DbScript> Schemas { get; set; }
    public List<DbScript> Seeds { get; set; }
  }
}
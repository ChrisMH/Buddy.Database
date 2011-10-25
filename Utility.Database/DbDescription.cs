using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Xml.Linq;

namespace Utility.Database
{
  public class DbDescription
  {
    public DbDescription(XElement root)
    {
      if (root.Element("ConnectionString") == null) throw new ArgumentException("ConnectionString element is missing", "ConnectionString");
      if (string.IsNullOrEmpty(root.Element("ConnectionString").Value)) throw new ArgumentException("ConnectionString element is empty", "ConnectionString");

      if (root.Element("ProviderName") == null) throw new ArgumentException("ProviderName element is missing", "ProviderName");
      if (string.IsNullOrEmpty(root.Element("ProviderName").Value)) throw new ArgumentException("ProviderName element is empty", "ProviderName");

      ConnectionString = new DbConnectionStringBuilder { ConnectionString = root.Element("ConnectionString").Value };
      ProviderName = root.Element("ProviderName").Value;
    }

    public DbConnectionStringBuilder ConnectionString { get; private set; }
    public string ProviderName { get; private set; }

    public IEnumerable<string> Schemas
    {
      get
      {
        foreach (var schema in schemas)
        {
          yield return schema.Load();
        }
      }
    }

    public IEnumerable<string> Seeds
    {
      get
      {
        foreach (var seed in seeds)
        {
          yield return seed.Load();
        }
      }
    }

    private List<DbScript> schemas = new List<DbScript>(); 
    private List<DbScript> seeds = new List<DbScript>();
  }
}
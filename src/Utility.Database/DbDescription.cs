using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Xml.Linq;
using MicrOrm;
using MicrOrm.Core;
using Utility;

namespace Utility.Database
{
  public class DbDescription
  {
    public DbDescription(XElement root, string baseDirectory = null)
    {
      if (root.Element("ConnectionName") == null) throw new ArgumentException("ConnectionName element is missing", "ConnectionName");
      if (string.IsNullOrEmpty(root.Element("ConnectionName").Value)) throw new ArgumentException("ConnectionName element is empty", "ConnectionName");

      ConnectionName = root.Element("ConnectionName").Value;

      root.Elements("Schema").ForEach(element => schemas.Add(new DbScript(element, baseDirectory)));
      root.Elements("Seed").ForEach(element => seeds.Add(new DbScript(element, baseDirectory)));
    }

    public string ConnectionName { get; private set; }

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

    internal List<DbScript> schemas = new List<DbScript>(); 
    internal List<DbScript> seeds = new List<DbScript>();
  }
}
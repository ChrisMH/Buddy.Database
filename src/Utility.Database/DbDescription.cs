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
    public DbDescription()
    {
      Schemas = new List<DbScript>();
      Seeds = new List<DbScript>();
    }

    public DbDescription(XElement root, string baseDirectory = null)
      : this()
    {
      if (root.Element("ConnectionName") == null) throw new ArgumentException("ConnectionName element is missing", "ConnectionName");
      if (string.IsNullOrEmpty(root.Element("ConnectionName").Value)) throw new ArgumentException("ConnectionName element is empty", "ConnectionName");

      ConnectionName = root.Element("ConnectionName").Value;

      root.Elements("Schema").ForEach(element => Schemas.Add(new DbScript(element, baseDirectory)));
      root.Elements("Seed").ForEach(element => Seeds.Add(new DbScript(element, baseDirectory)));
    }

    public string ConnectionName { get; set; }

    public List<DbScript> Schemas { get; set; }
    public List<DbScript> Seeds { get; set; }
  }
}
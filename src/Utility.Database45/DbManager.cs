using System;
using System.Xml.Linq;

namespace Utility.Database
{
  public static class DbManager
  {
    /// <summary>
    /// Create from a DbManager node containing a type
    /// 
    /// <DbManager type="assembly.class, assembly">
    ///   <DbDescription type="assembly.class, assembly>
    ///     <!-- see DbDescription.cs -->
    ///   </DbDescription>
    /// </DbManager>
    /// 
    /// </summary>
    /// <param name="xmlRoot"></param>
    /// <returns></returns>
    public static IDbManager Create(string xmlRoot, string baseDirectory = null)
    {
      if (string.IsNullOrWhiteSpace(xmlRoot)) throw new ArgumentException("xmlRoot is invalid", "xmlRoot");


      var root = XElement.Parse(xmlRoot);

      var typeAttribute = root.Attribute("type");
      if (typeAttribute == null) throw new ArgumentException("Root element is missing the required 'type' attribute", "xmlRoot");

      IDbManager dbManager;
      try
      {
        dbManager = new ReflectionType(typeAttribute.Value).CreateObject<IDbManager>();
      }
      catch (Exception e)
      {
        throw new ArgumentException(string.Format("Could not create database manager type '{0}'", typeAttribute.Value), "xmlRoot", e);
      }

      dbManager.Description = DbDescription.Create(root.FirstNode.ToString(), baseDirectory);

      return dbManager;
    }
  }
}
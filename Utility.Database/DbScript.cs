using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Xml.Linq;

namespace Utility.Database
{
  public enum ScriptType
  {
    file,
    resource
  };

  internal class DbScript
  {
    public DbScript(XElement root, string baseDirectory = null)
    {
      BaseDirectory = baseDirectory;
      if (string.IsNullOrEmpty(baseDirectory))
      {
        BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
      }

      if (!Directory.Exists(BaseDirectory)) throw new ArgumentException(string.Format("Directory '{0}' does not exist", BaseDirectory), "baseDirectory");

      if (root.Attribute("type") == null) throw new ArgumentException("type attribute is missing", "type");

      ScriptType scriptType;
      if (!Enum.TryParse(root.Attribute("type").Value, false, out scriptType)) throw new ArgumentException(string.Format("type attribute value is invalid: {0}", root.Attribute("type")), "type");
      ScriptType = scriptType;

      ScriptValue = root.Value;
      if (string.IsNullOrEmpty(ScriptValue)) throw new ArgumentException(string.Format("{0} element is empty", root.Name.LocalName), root.Name.LocalName);
    }

    public string Load()
    {
      if (ScriptType == ScriptType.file)
      {
        var fileName = ScriptValue;
        if (!File.Exists(fileName))
        {
          fileName = Path.Combine(BaseDirectory, ScriptValue);
          if (!File.Exists(fileName))
          {
            throw new FileNotFoundException("File not found", ScriptValue);
          }
        }
        
        using(var reader = new StreamReader(File.OpenRead(fileName)))
        {
          return reader.ReadToEnd();
        }
      }
      else
      {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
          var resourceStream = assembly.GetManifestResourceStream(ScriptValue);
          if (resourceStream != null)
          {
            using (var reader = new StreamReader(resourceStream))
            {
              return reader.ReadToEnd();
            }
          }
        }

        throw new MissingManifestResourceException(string.Format("Could not find resource '{0}'", ScriptValue));
      }
    }

    public string BaseDirectory { get; private set; }
    public ScriptType ScriptType { get; private set; }
    public string ScriptValue { get; private set; }
  }
}
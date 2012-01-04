using System;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Xml.Linq;

namespace Utility.Database
{
  public enum ScriptType
  {
    File,
    Resource,
    Literal,
    Runnable
  };

  public class DbScript
  {
    public DbScript()
    {
      GetBaseDirectory = () => AppDomain.CurrentDomain.BaseDirectory;
    }

    public string Load()
    {
      switch (ScriptType)
      {
        case ScriptType.File:
          {
            var fileName = ScriptValue;

            if (Path.IsRelative(fileName))
            {
              fileName = Path.Combine(GetBaseDirectory.Invoke(), ScriptValue);
            }

            if (!Path.FileExists(fileName))
            {
              throw new FileNotFoundException("File not found", ScriptValue);
            }

            using (var reader = new StreamReader(File.OpenRead(fileName)))
            {
              return reader.ReadToEnd();
            }
          }
        case ScriptType.Resource:
          {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
              if (assembly.IsDynamic)
              {
                // GetManifestResourceStream will not work for dynamic assemblies
                continue;
              }
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
        case ScriptType.Literal:
          {
            return ScriptValue;
          }
        default:
          {
            throw new ArgumentException(string.Format("ScriptType is not loadable : {0}", ScriptType), "ScriptType");
          }
      }
    }


    public void Run(IDbConnectionInfo connectionInfo)
    {
      if (ScriptType != ScriptType.Runnable) throw new ArgumentException(string.Format("ScriptType is not runnable : {0}", ScriptType), "ScriptType");

      try
      {
        var rType = new ReflectionType(ScriptValue);
        var runMethod = rType.CreateType().GetMethod("Run");
        if (runMethod == null)
          throw new ArgumentException(string.Format("Could not find a method named 'Run' for ScriptValue : {0}", ScriptValue), "ScriptValue");
        if (runMethod.ReturnType != typeof (void))
          throw new ArgumentException(string.Format("Run method does not have return type 'void' for ScriptValue : {0}", ScriptValue), "ScriptValue");
        var parameters = runMethod.GetParameters();
        if (parameters.Length != 1 || parameters[0].ParameterType != typeof (IDbConnectionInfo))
          throw new ArgumentException(
            string.Format("Run method does not have a single parameter of type 'IDbConnectionInfo' for ScriptValue : {0}", ScriptValue), "ScriptValue");

        runMethod.Invoke(rType.CreateObject(), BindingFlags.InvokeMethod, null, new object[] {connectionInfo}, null);
      }
      catch (Exception e)
      {
        if (e.GetType() == typeof (ArgumentException)) throw;
        throw new ArgumentException(string.Format("ScriptValue could not be run : {0} : {1} : {2}", ScriptValue, e.GetType(), e.Message), "ScriptValue", e);
      }
    }

    public Func<string> GetBaseDirectory { get; set; }
    public ScriptType ScriptType { get; set; }
    public string ScriptValue { get; set; }

    public string XmlRoot
    {
      set
      {
        var root = XElement.Parse(value);
        if (root.Attribute("type") == null) throw new ArgumentException("type attribute is missing", "type");

        ScriptType scriptType;
        if (!Enum.TryParse(root.Attribute("type").Value, false, out scriptType))
          throw new ArgumentException(string.Format("type attribute value is invalid: {0}", root.Attribute("type")), "type");
        ScriptType = scriptType;

        ScriptValue = root.Value;
        if (string.IsNullOrEmpty(ScriptValue)) throw new ArgumentException(string.Format("{0} element is empty", root.Name.LocalName), root.Name.LocalName);
      }
    }
  }
}
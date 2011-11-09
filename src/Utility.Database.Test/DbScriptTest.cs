using System;
using System.Resources;
using System.Xml.Linq;
using NUnit.Framework;

namespace Utility.Database.Test
{
  public class DbScriptTest
  {
    [Test]
    public void EmptyBaseDirectoryUsesAppDomainBaseDirectory()
    {
      var root = XElement.Parse(DbScripts.RelativeFileSchema);

      var result = new DbScript(root);

      Assert.AreEqual(AppDomain.CurrentDomain.BaseDirectory, result.BaseDirectory);
    }

    [Test]
    public void InvalidBaseDirectoryThrows()
    {
      var root = XElement.Parse(DbScripts.RelativeFileSchema);

      var e = Assert.Throws<ArgumentException>(() => new DbScript(root, "d:\\invalid"));
      Assert.AreEqual("baseDirectory", e.ParamName);

    }
    [TestCase(DbScripts.MissingSchemaType)]
    [TestCase(DbScripts.MissingSeedType)]
    public void MissingTypeThrows(string script)
    {
      var root = XElement.Parse(script);

      var e = Assert.Throws<ArgumentException>(() => new DbScript(root));
      Assert.AreEqual("type", e.ParamName);
    }

    [TestCase(DbScripts.InvalidSchemaType)]
    [TestCase(DbScripts.InvalidSeedType)]
    public void InvalidTypeThrows(string script)
    {
      var root = XElement.Parse(script);

      var e = Assert.Throws<ArgumentException>(() => new DbScript(root));
      Assert.AreEqual("type", e.ParamName);
    }

    [TestCase(DbScripts.EmptyFileSchema, "Schema")]
    [TestCase(DbScripts.EmptyResourceSchema, "Schema")]
    [TestCase(DbScripts.EmptyFileSeed, "Seed")]
    [TestCase(DbScripts.EmptyResourceSeed, "Seed")]
    public void EmptyValueThrows(string script, string paramName)
    {
      var root = XElement.Parse(script);

      var e = Assert.Throws<ArgumentException>(() => new DbScript(root));
      Assert.AreEqual(paramName, e.ParamName);
    }

    [TestCase(DbScripts.RelativeFileSchema, ScriptType.file, "..\\..\\Resources\\schema.txt")]
    [TestCase(DbScripts.AbsoluteFileSchema, ScriptType.file, "d:\\DevP\\Utility.Database\\src\\Utility.Database.Test\\Resources\\schema.txt")]
    [TestCase(DbScripts.ResourceSchema, ScriptType.resource, "Utility.Database.Test.Resources.schema.txt")]
    [TestCase(DbScripts.RelativeFileSeed, ScriptType.file, "..\\..\\Resources\\seed.txt")]
    [TestCase(DbScripts.AbsoluteFileSeed, ScriptType.file, "d:\\DevP\\Utility.Database\\src\\Utility.Database.Test\\Resources\\seed.txt")]
    [TestCase(DbScripts.ResourceSeed, ScriptType.resource, "Utility.Database.Test.Resources.seed.txt")]
    public void ScriptIsParsedCorrectly(string script, ScriptType scriptType, string scriptValue)
    {
      var root = XElement.Parse(script);

      var result = new DbScript(root);

      Assert.AreEqual(scriptType, result.ScriptType);
      Assert.AreEqual(scriptValue, result.ScriptValue);
    }

    [TestCase(DbScripts.RelativeMissingFileSchema, "schema.txt")]
    [TestCase(DbScripts.AbsoluteMissingFileSchema, "d:\\schema.txt")]
    [TestCase(DbScripts.RelativeMissingFileSeed, "seed.txt")]
    [TestCase(DbScripts.AbsoluteMissingFileSeed, "d:\\seed.txt")]
    public void MissingFileThrows(string script, string fileName)
    {
      var root = XElement.Parse(script);

      var result = new DbScript(root);

      var e = Assert.Throws<System.IO.FileNotFoundException>(() => result.Load());
      Assert.AreEqual(fileName, e.FileName);
    }

    [TestCase(DbScripts.MissingResourceSchema)]
    [TestCase(DbScripts.MissingResourceSeed)]
    public void MissingResourceThrows(string script)
    {
      var root = XElement.Parse(script);

      var result = new DbScript(root);

      Assert.Throws<MissingManifestResourceException>(() => result.Load());
    }

    [TestCase(DbScripts.RelativeFileSchema, "schema")]
    [TestCase(DbScripts.AbsoluteFileSchema, "schema")]
    [TestCase(DbScripts.ResourceSchema, "schema")]
    [TestCase(DbScripts.RelativeFileSeed, "seed")]
    [TestCase(DbScripts.AbsoluteFileSeed, "seed")]
    [TestCase(DbScripts.ResourceSeed, "seed")]
    public void ScriptIsLoaded(string script, string content)
    {
      var root = XElement.Parse(script);

      var result = new DbScript(root).Load();

      Assert.AreEqual(content, result);
    }
  }
}
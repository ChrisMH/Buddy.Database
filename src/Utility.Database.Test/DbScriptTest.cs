using System;
using System.IO;
using System.Resources;
using NUnit.Framework;

namespace Utility.Database.Test
{
  public class DbScriptTest
  {
    [Test]
    public void EmptyBaseDirectoryUsesAppDomainBaseDirectory()
    {
      var result = new DbScript {XmlRoot = DbScripts.RelativeFileSchema};

      Assert.That(result.GetBaseDirectory.Invoke(), Is.EqualTo(AppDomain.CurrentDomain.BaseDirectory));
    }

    [Test]
    public void InvalidBaseDirectoryThrows()
    {
      Assert.That(() => new DbScript {XmlRoot = DbScripts.RelativeFileSchema, GetBaseDirectory = () => "d:\\invalid"}.Load(),
                  Throws.InstanceOf<FileNotFoundException>());
    }

    [TestCase(DbScripts.MissingSchemaType)]
    [TestCase(DbScripts.MissingSeedType)]
    public void MissingTypeThrows(string script)
    {
      Assert.That(() => new DbScript {XmlRoot = script},
                  Throws.ArgumentException.With.Property("ParamName").EqualTo("type"));
    }

    [TestCase(DbScripts.InvalidSchemaType)]
    [TestCase(DbScripts.InvalidSeedType)]
    public void InvalidTypeThrows(string script)
    {
      Assert.That(() => new DbScript {XmlRoot = script},
                  Throws.ArgumentException.With.Property("ParamName").EqualTo("type"));
    }

    [TestCase(DbScripts.EmptyFileSchema, "Schema")]
    [TestCase(DbScripts.EmptyResourceSchema, "Schema")]
    [TestCase(DbScripts.EmptyLiteralSchema, "Schema")]
    [TestCase(DbScripts.EmptyFileSeed, "Seed")]
    [TestCase(DbScripts.EmptyResourceSeed, "Seed")]
    [TestCase(DbScripts.EmptyLiteralSeed, "Seed")]
    public void EmptyValueThrows(string script, string paramName)
    {
      Assert.That(() => new DbScript {XmlRoot = script},
                  Throws.ArgumentException.With.Property("ParamName").EqualTo(paramName));
    }

    [TestCase(DbScripts.RelativeFileSchema, ScriptType.File, "..\\..\\Resources\\schema.txt")]
    [TestCase(DbScripts.AbsoluteFileSchema, ScriptType.File, "d:\\DevP\\Utility.Database\\src\\Utility.Database.Test\\Resources\\schema.txt")]
    [TestCase(DbScripts.ResourceSchema, ScriptType.Resource, "Utility.Database.Test.Resources.schema.txt")]
    [TestCase(DbScripts.RelativeFileSeed, ScriptType.File, "..\\..\\Resources\\seed.txt")]
    [TestCase(DbScripts.AbsoluteFileSeed, ScriptType.File, "d:\\DevP\\Utility.Database\\src\\Utility.Database.Test\\Resources\\seed.txt")]
    [TestCase(DbScripts.ResourceSeed, ScriptType.Resource, "Utility.Database.Test.Resources.seed.txt")]
    [TestCase(DbScripts.LiteralSchema, ScriptType.Literal, "CREATE SCHEMA literal;CREATE TABLE literal.table (id integer NOT NULL);")]
    [TestCase(DbScripts.LiteralSeed, ScriptType.Literal, "INSERT INTO literal.table VALUES(1);INSERT INTO literal.table VALUES(2);")]
    [TestCase(DbScripts.RunnableSchema, ScriptType.Runnable, "Utility.Database.Test.Runnable, Utility.Database.Test")]
    [TestCase(DbScripts.RunnableSeed, ScriptType.Runnable, "Utility.Database.Test.Runnable, Utility.Database.Test")]
    public void ScriptIsParsedCorrectly(string script, ScriptType scriptType, string scriptValue)
    {
      var result = new DbScript {XmlRoot = script};

      Assert.That(result.ScriptType, Is.EqualTo(scriptType));
      Assert.That(result.ScriptValue, Is.EqualTo(scriptValue));
    }

    [TestCase(DbScripts.RelativeMissingFileSchema, "schema.txt")]
    [TestCase(DbScripts.AbsoluteMissingFileSchema, "d:\\schema.txt")]
    [TestCase(DbScripts.RelativeMissingFileSeed, "seed.txt")]
    [TestCase(DbScripts.AbsoluteMissingFileSeed, "d:\\seed.txt")]
    public void MissingFileThrows(string script, string fileName)
    {
      var result = new DbScript {XmlRoot = script};

      Assert.That(() => result.Load(), Throws.InstanceOf<FileNotFoundException>());
    }

    [TestCase(DbScripts.MissingResourceSchema)]
    [TestCase(DbScripts.MissingResourceSeed)]
    public void MissingResourceThrows(string script)
    {
      var result = new DbScript {XmlRoot = script};

      Assert.That(() => result.Load(), Throws.InstanceOf<MissingManifestResourceException>());
    }


    [TestCase(DbScripts.RelativeFileSchema, "schema")]
    [TestCase(DbScripts.AbsoluteFileSchema, "schema")]
    [TestCase(DbScripts.ResourceSchema, "schema")]
    [TestCase(DbScripts.RelativeFileSeed, "seed")]
    [TestCase(DbScripts.AbsoluteFileSeed, "seed")]
    [TestCase(DbScripts.ResourceSeed, "seed")]
    public void LoadableScriptIsLoaded(string script, string content)
    {
      var result = new DbScript {XmlRoot = script}.Load();

      Assert.That(result, Is.EqualTo(content));
    }

    [TestCase(DbScripts.RelativeFileSchema, "schema")]
    [TestCase(DbScripts.AbsoluteFileSchema, "schema")]
    [TestCase(DbScripts.ResourceSchema, "schema")]
    [TestCase(DbScripts.RelativeFileSeed, "seed")]
    [TestCase(DbScripts.AbsoluteFileSeed, "seed")]
    [TestCase(DbScripts.ResourceSeed, "seed")]
    public void LoadableScriptThrowsIfRun(string script, string content)
    {
      Assert.That(() => new DbScript {XmlRoot = script}.Run(new DbConnectionInfo()),
                  Throws.ArgumentException.With.Property("ParamName").EqualTo("ScriptType"));
    }

    [TestCase(DbScripts.RunnableSchema)]
    [TestCase(DbScripts.RunnableSeed)]
    public void RunnableScriptIsRun(string script)
    {
      Assert.That((() => new DbScript {XmlRoot = script}.Run(new DbConnectionInfo())),
                  Throws.ArgumentException.With.InnerException.InnerException.Message.EqualTo("Method was called"));
    }

    [TestCase(DbScripts.RunnableSchema)]
    [TestCase(DbScripts.RunnableSeed)]
    public void RunnableScriptThrowsIfLoaded(string script)
    {
      Assert.That(() => new DbScript {XmlRoot = script}.Load(),
                  Throws.ArgumentException.With.Property("ParamName").EqualTo("ScriptType"));
    }

    [TestCase(DbScripts.RunnableWithMissingClass)]
    [TestCase(DbScripts.RunnableWithInvalidMethodName)]
    [TestCase(DbScripts.RunnableWithInvalidReturnType)]
    [TestCase(DbScripts.RunnableWithInvalidMethodSignature)]
    public void RunThrowsIfItCantBeRun(string script)
    {
      Assert.That(() => new DbScript {XmlRoot = script}.Run(new DbConnectionInfo()),
                  Throws.ArgumentException.With.Property("ParamName").EqualTo("ScriptValue"));
    }
  }
}
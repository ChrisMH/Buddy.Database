namespace Utility.Database.Test
{
  public static class DbScripts
  {
    public const string MissingSchemaType = "<Schema></Schema>";
    public const string MissingSeedType = "<Seed></Seed>";

    public const string InvalidSchemaType = "<Schema type=\"Invalid\"></Schema>";
    public const string InvalidSeedType = "<Seed type=\"Invalid\"></Seed>";

    public const string EmptyFileSchema = "<Schema type=\"File\"></Schema>";
    public const string EmptyResourceSchema = "<Schema type=\"Resource\"></Schema>";
    public const string EmptyLiteralSchema = "<Schema type=\"Literal\"></Schema>";

    public const string EmptyFileSeed = "<Seed type=\"File\"></Seed>";
    public const string EmptyResourceSeed = "<Seed type=\"Resource\"></Seed>";
    public const string EmptyLiteralSeed = "<Seed type=\"Literal\"></Seed>";

    public const string RelativeMissingFileSchema = "<Schema type=\"File\">schema.txt</Schema>";
    public const string AbsoluteMissingFileSchema = "<Schema type=\"File\">d:\\schema.txt</Schema>";
    public const string RelativeMissingFileSeed = "<Schema type=\"File\">seed.txt</Schema>";
    public const string AbsoluteMissingFileSeed = "<Schema type=\"File\">d:\\seed.txt</Schema>";
    public const string MissingResourceSchema = "<Schema type=\"Resource\">schema.txt</Schema>";
    public const string MissingResourceSeed = "<Schema type=\"Resource\">schema.txt</Schema>";

    public const string RelativeFileSchema = "<Schema type=\"File\">..\\..\\Resources\\schema.txt</Schema>";
    public const string AbsoluteFileSchema = "<Schema type=\"File\">d:\\DevP\\Utility.Database\\src\\Utility.Database.Test\\Resources\\schema.txt</Schema>";
    public const string ResourceSchema = "<Schema type=\"Resource\">Utility.Database.Test.Resources.schema.txt</Schema>";
    public const string RelativeFileSeed = "<Schema type=\"File\">..\\..\\Resources\\seed.txt</Schema>";
    public const string AbsoluteFileSeed = "<Schema type=\"File\">d:\\DevP\\Utility.Database\\src\\Utility.Database.Test\\Resources\\seed.txt</Schema>";
    public const string ResourceSeed = "<Schema type=\"Resource\">Utility.Database.Test.Resources.seed.txt</Schema>";
    public const string LiteralSchema = "<Schema type=\"Literal\">CREATE SCHEMA literal;CREATE TABLE literal.table (id integer NOT NULL);</Schema>";
    public const string LiteralSeed = "<Seed type=\"Literal\">INSERT INTO literal.table VALUES(1);INSERT INTO literal.table VALUES(2);</Seed>";
    public const string RunnableSchema = "<Schema type=\"Runnable\">Utility.Database.Test.Runnable, Utility.Database.Test</Schema>";
    public const string RunnableSeed = "<Seed type=\"Runnable\">Utility.Database.Test.Runnable, Utility.Database.Test</Seed>";



    public const string RunnableWithMissingClass = "<Schema type=\"Runnable\">Utility.Database.Test.RunnableWithMissingClass, Utility.Database.Test</Schema>";
    public const string RunnableWithInvalidMethodName = "<Schema type=\"Runnable\">Utility.Database.Test.RunnableWithInvalidMethodName, Utility.Database.Test</Schema>";
    public const string RunnableWithInvalidReturnType = "<Schema type=\"Runnable\">Utility.Database.Test.RunnableWithInvalidReturnType, Utility.Database.Test</Schema>";
    public const string RunnableWithInvalidMethodSignature = "<Schema type=\"Runnable\">Utility.Database.Test.RunnableWithInvalidMethodSignature, Utility.Database.Test</Schema>";
  }
}
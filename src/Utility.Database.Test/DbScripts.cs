namespace Utility.Database.Test
{
  public static class DbScripts
  {
    public const string MissingSchemaType = "<Schema></Schema>";
    public const string MissingSeedType = "<Seed></Seed>";

    public const string InvalidSchemaType = "<Schema type=\"invalid\"></Schema>";
    public const string InvalidSeedType = "<Seed type=\"invalid\"></Seed>";

    public const string EmptyFileSchema = "<Schema type=\"file\"></Schema>";
    public const string EmptyResourceSchema = "<Schema type=\"resource\"></Schema>";

    public const string EmptyFileSeed = "<Seed type=\"file\"></Seed>";
    public const string EmptyResourceSeed = "<Seed type=\"resource\"></Seed>";

    public const string RelativeMissingFileSchema = "<Schema type=\"file\">schema.txt</Schema>";
    public const string AbsoluteMissingFileSchema = "<Schema type=\"file\">d:\\schema.txt</Schema>";
    public const string RelativeMissingFileSeed = "<Schema type=\"file\">seed.txt</Schema>";
    public const string AbsoluteMissingFileSeed = "<Schema type=\"file\">d:\\seed.txt</Schema>";
    public const string MissingResourceSchema = "<Schema type=\"resource\">schema.txt</Schema>";
    public const string MissingResourceSeed = "<Schema type=\"resource\">schema.txt</Schema>";

    public const string RelativeFileSchema = "<Schema type=\"file\">..\\..\\Resources\\schema.txt</Schema>";
    public const string AbsoluteFileSchema = "<Schema type=\"file\">d:\\DevP\\Utility.Database\\src\\Utility.Database.Test\\Resources\\schema.txt</Schema>";
    public const string ResourceSchema = "<Schema type=\"resource\">Utility.Database.Test.Resources.schema.txt</Schema>";
    public const string RelativeFileSeed = "<Schema type=\"file\">..\\..\\Resources\\seed.txt</Schema>";
    public const string AbsoluteFileSeed = "<Schema type=\"file\">d:\\DevP\\Utility.Database\\src\\Utility.Database.Test\\Resources\\seed.txt</Schema>";
    public const string ResourceSeed = "<Schema type=\"resource\">Utility.Database.Test.Resources.seed.txt</Schema>";
  }
}
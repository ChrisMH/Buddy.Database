namespace Utility.Database.Test
{
  public static class DbDescriptions
  {
    public const string Empty = "<DbDescription></DbDescription>";
    public const string EmptyConnectionName = "<DbDescription><ConnectionName></ConnectionName></DbDescription>";
    public const string InvalidConnectionName = "<DbDescription><ConnectionName>InvalidConnectionName</ConnectionName></DbDescription>";
    
    public const string MinimumValid = "<DbDescription><ConnectionName>ConnectionName</ConnectionName></DbDescription>";

    public const string SingleFileSchema = "<DbDescription>" +
                                           "<ConnectionName>ConnectionName</ConnectionName>" +
                                           "<Schema type=\"file\">d:\\DevP\\Utility.Database\\src\\Utility.Database.Test\\Resources\\schema.txt</Schema>" +
                                           "</DbDescription>";

    public const string SingleResourceSchema = "<DbDescription>" +
                                               "<ConnectionName>ConnectionName</ConnectionName>" +
                                               "<Schema type=\"resource\">Utility.Database.Test.Resources.schema.txt</Schema>" +
                                               "</DbDescription>";

    public const string SingleFileSeed = "<DbDescription>" +
                                         "<ConnectionName>ConnectionName</ConnectionName>" +
                                         "<Seed type=\"file\">d:\\DevP\\Utility.Database\\src\\Utility.Database.Test\\Resources\\seed.txt</Seed>" +
                                         "</DbDescription>";

    public const string SingleResourceSeed = "<DbDescription>" +
                                             "<ConnectionName>ConnectionName</ConnectionName>" +
                                             "<Seed type=\"resource\">Utility.Database.Test.Resources.seed.txt</Seed>" +
                                             "</DbDescription>";


    public const string SchemasAndSeeds = "<DbDescription>" +
                                          "<ConnectionName>ConnectionName</ConnectionName>" +
                                          "<Schema type=\"file\">d:\\DevP\\Utility.Database\\src\\Utility.Database.Test\\Resources\\schema.txt</Schema>" +
                                          "<Schema type=\"resource\">Utility.Database.Test.Resources.schema.txt</Schema>" +
                                          "<Seed type=\"file\">d:\\DevP\\Utility.Database\\src\\Utility.Database.Test\\Resources\\seed.txt</Seed>" +
                                          "<Seed type=\"resource\">Utility.Database.Test.Resources.seed.txt</Seed>" +
                                          "</DbDescription>";

    public const string RelativeFileSchema = "<DbDescription>" +
                                             "<ConnectionName>ConnectionName</ConnectionName>" +
                                             "<Schema type=\"file\">Resources\\schema.txt</Schema>" +
                                             "<Seed type=\"file\">Resources\\seed.txt</Seed>" +
                                             "</DbDescription>";
  }
}
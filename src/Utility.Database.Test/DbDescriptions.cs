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
                                           "<Schema type=\"File\">d:\\DevP\\Utility.Database\\src\\Utility.Database.Test\\Resources\\schema.txt</Schema>" +
                                           "</DbDescription>";

    public const string SingleResourceSchema = "<DbDescription>" +
                                               "<ConnectionName>ConnectionName</ConnectionName>" +
                                               "<Schema type=\"Resource\">Utility.Database.Test.Resources.schema.txt</Schema>" +
                                               "</DbDescription>";

    public const string SingleFileSeed = "<DbDescription>" +
                                         "<ConnectionName>ConnectionName</ConnectionName>" +
                                         "<Seed type=\"File\">d:\\DevP\\Utility.Database\\src\\Utility.Database.Test\\Resources\\seed.txt</Seed>" +
                                         "</DbDescription>";

    public const string SingleResourceSeed = "<DbDescription>" +
                                             "<ConnectionName>ConnectionName</ConnectionName>" +
                                             "<Seed type=\"Resource\">Utility.Database.Test.Resources.seed.txt</Seed>" +
                                             "</DbDescription>";


    public const string SchemasAndSeeds = "<DbDescription>" +
                                          "<ConnectionName>ConnectionName</ConnectionName>" +
                                          "<Schema type=\"File\">d:\\DevP\\Utility.Database\\src\\Utility.Database.Test\\Resources\\schema.txt</Schema>" +
                                          "<Schema type=\"Resource\">Utility.Database.Test.Resources.schema.txt</Schema>" +
                                          "<Seed type=\"File\">d:\\DevP\\Utility.Database\\src\\Utility.Database.Test\\Resources\\seed.txt</Seed>" +
                                          "<Seed type=\"Resource\">Utility.Database.Test.Resources.seed.txt</Seed>" +
                                          "</DbDescription>";

    public const string RelativeFileSchema = "<DbDescription>" +
                                             "<ConnectionName>ConnectionName</ConnectionName>" +
                                             "<Schema type=\"File\">Resources\\schema.txt</Schema>" +
                                             "<Seed type=\"File\">Resources\\seed.txt</Seed>" +
                                             "</DbDescription>";
                                             
    public const string PgMinimumValidWithInvalidTemplateName = "<DbDescription><ConnectionName>ConnectionName</ConnectionName><TemplateName></TemplateName></DbDescription>";
    public const string PgMinimumValidWithTemplateName = "<DbDescription><ConnectionName>ConnectionName</ConnectionName><TemplateName>template_postgis</TemplateName></DbDescription>";
  }
}
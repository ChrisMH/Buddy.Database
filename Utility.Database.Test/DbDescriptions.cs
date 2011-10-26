namespace Utility.Database.Test
{
  public static class DbDescriptions
  {
    public static string Empty = "<DbDescription></DbDescription>";
    public static string EmptyConnectionString = "<DbDescription><ConnectionString></ConnectionString></DbDescription>";
    public static string MissingProviderName = "<DbDescription><ConnectionString>database=test</ConnectionString></DbDescription>";
    public static string EmptyProviderName = "<DbDescription><ConnectionString>database=test</ConnectionString><ProviderName></ProviderName></DbDescription>";

    public static string MinimumValid = "<DbDescription><ConnectionString>database=test</ConnectionString><ProviderName>Provider</ProviderName></DbDescription>";

    public static string SingleFileSchema = "<DbDescription>" +
                                            "<ConnectionString>database=test</ConnectionString><ProviderName>Provider</ProviderName>" +
                                            "<Schema type=file>d:\\directory\\desc.xml</Schema>" +
                                            "</DbDescription>";

    public static string SingleResourceSchema = "<DbDescription>" +
                                                "<ConnectionString>database=test</ConnectionString><ProviderName>Provider</ProviderName>" +
                                                "<Schema type=resource>desc</Schema>" +
                                                "</DbDescription>";
  }
}
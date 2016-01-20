namespace Utility.Database.Test
{
  public static class DbDescriptions
  {
    public const string Empty = "<DbDescription></DbDescription>";

    // Valid Connection
    public const string ConnectionWithConnectionStringName = "<DbDescription>" +
                                                             "<Connection>" +
                                                             "<ConnectionStringName>Valid</ConnectionStringName>" +
                                                             "</Connection>" +
                                                             "</DbDescription>";
                                                             
    public const string ConnectionWithConnectionStringAndProviderName = "<DbDescription>" +
                                                                        "<Connection>" +
                                                                        "<ConnectionString>server=server</ConnectionString>" +
                                                                        "<Provider>System.Data.SqlClient</Provider>" +
                                                                        "</Connection>" +
                                                                        "</DbDescription>";

    public const string ConnectionWithConnectionStringAndProviderType = 
      "<DbDescription>" +
        "<Connection>" +
          "<ConnectionString>server=server</ConnectionString>" +
          "<Provider>System.Data.SqlClient.SqlClientFactory, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</Provider>" +
        "</Connection>" +
      "</DbDescription>";
              
    
    // DbDescription with type                                                                        
    public const string ExplicitDbDescriptionType = 
      "<DbDescription type=\"Utility.Database.Test.TestDbDescription, Utility.Database.Test\">" +
        "<Connection>" +
          "<ConnectionStringName>Valid</ConnectionStringName>" +
        "</Connection>" +
      "</DbDescription>";

    public const string ImplicitDbDescriptionType =
      "<DbDescription>" +
        "<Connection>" +
          "<ConnectionStringName>Valid</ConnectionStringName>" +
        "</Connection>" +
      "</DbDescription>";

    // Invalid Connection
    public const string EmptyConnection = "<DbDescription><Connection></Connection></DbDescription>";


    public const string ConnectionWithInvalidConnectionStringName = "<DbDescription>" +
                                                                    "<Connection>" +
                                                                    "<ConnectionStringName>InvalidConnectionName</ConnectionStringName>" +
                                                                    "</Connection>" +
                                                                    "</DbDescription>";


    public const string ConnectionWithConnectionString = "<DbDescription>" +
                                                         "<Connection>" +
                                                         "<ConnectionString>server=server</ConnectionString>" +
                                                         "</Connection>" +
                                                         "</DbDescription>";

    public const string ConnectionWithProviderName = "<DbDescription>" +
                                                     "<Connection>" +
                                                     "<ProviderName>System.Data.SqlClient</ProviderName>" +
                                                     "</Connection>" +
                                                     "</DbDescription>";
                                                     
    public const string ConnectionWithInvalidProviderName = "<DbDescription>" +
                                                                        "<Connection>" +
                                                                        "<ConnectionString>server=server</ConnectionString>" +
                                                                        "<ProviderName>Invalid.Provider.Name</ProviderName>" +
                                                                        "</Connection>" +
                                                                        "</DbDescription>";

    // Schemas and Seeds
    public const string SingleFileSchema = "<DbDescription>" +
                                           "<Schema type=\"File\">d:\\DevP\\Utility.Database\\src\\Utility.Database.Test\\Resources\\schema.txt</Schema>" +
                                           "</DbDescription>";

    public const string SingleResourceSchema = "<DbDescription>" +
                                               "<Schema type=\"Resource\">Utility.Database.Test.Resources.schema.txt</Schema>" +
                                               "</DbDescription>";

    public const string SingleFileSeed = "<DbDescription>" +
                                         "<Seed type=\"File\">d:\\DevP\\Utility.Database\\src\\Utility.Database.Test\\Resources\\seed.txt</Seed>" +
                                         "</DbDescription>";

    public const string SingleResourceSeed = "<DbDescription>" +
                                             "<Seed type=\"Resource\">Utility.Database.Test.Resources.seed.txt</Seed>" +
                                             "</DbDescription>";


    public const string SchemasAndSeeds = "<DbDescription>" +
                                          "<Schema type=\"File\">d:\\DevP\\Utility.Database\\src\\Utility.Database.Test\\Resources\\schema.txt</Schema>" +
                                          "<Schema type=\"Resource\">Utility.Database.Test.Resources.schema.txt</Schema>" +
                                          "<Seed type=\"File\">d:\\DevP\\Utility.Database\\src\\Utility.Database.Test\\Resources\\seed.txt</Seed>" +
                                          "<Seed type=\"Resource\">Utility.Database.Test.Resources.seed.txt</Seed>" +
                                          "</DbDescription>";

    public const string RelativeFileSchema = "<DbDescription>" +
                                             "<Schema type=\"File\">Resources\\schema.txt</Schema>" +
                                             "<Seed type=\"File\">Resources\\seed.txt</Seed>" +
                                             "</DbDescription>";
    
  }
}
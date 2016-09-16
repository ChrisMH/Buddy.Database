namespace Utility.Database.Mock.Test
{
  public static class DbDescriptions
  {
    public const string Valid = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" +
                                "<DbDescription>" +
                                "<Schema type=\"Runnable\">Utility.Database.Mock.Test.MockSchema, Utility.Database.Mock.Test</Schema>" +
                                "<Seed type=\"Runnable\">Utility.Database.Mock.Test.MockSeed1, Utility.Database.Mock.Test</Seed>" +
                                "<Seed type=\"Runnable\">Utility.Database.Mock.Test.MockSeed2, Utility.Database.Mock.Test</Seed>" +
                                "</DbDescription>";

    public const string InvalidSchemaType = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" +
                                "<DbDescription>" +
                                "<Schema type=\"Literal\">literal schema</Schema>" +
                                "</DbDescription>";

    public const string InvalidSeedType = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" +
                                "<DbDescription>" +
                                "<Seed type=\"Literal\">literal seed</Seed>" +
                                "</DbDescription>";
  }
}
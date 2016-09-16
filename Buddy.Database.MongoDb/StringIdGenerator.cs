using System;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Utility.Database.MongoDb
{
  public class StringIdGenerator : IIdGenerator
  {
    public object GenerateId(object container, object document)
    {
      return ObjectId.GenerateNewId().ToString();
    }

    public bool IsEmpty(object id)
    {
      return string.IsNullOrWhiteSpace((string)id);
    }
  }
}

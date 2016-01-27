using System;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Utility.Database.MongoDb
{
  public class ReflectionTypeSerializer : BsonBaseSerializer
  {
    public override object Deserialize(BsonReader bsonReader, Type nominalType, Type actualType, IBsonSerializationOptions options)
    {
      var aqName = bsonReader.ReadString();
      return new ReflectionType(aqName);
    }

    public override object Deserialize(BsonReader bsonReader, Type nominalType, IBsonSerializationOptions options)
    {
      var aqName = bsonReader.ReadString();
      return new ReflectionType(aqName);
    }

    public override void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
    {
      bsonWriter.WriteString(((ReflectionType)value).ToString());
    }
  }
}

using System;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Utility.Database.MongoDb
{
  public class StringIdSerializer : BsonBaseSerializer
  {
    public override object Deserialize(BsonReader bsonReader, Type nominalType, Type actualType, IBsonSerializationOptions options)
    {
      // read an ObjectId from the database but convert it to a string before returning it
      int timestamp, machine, increment;
      short pid;
      bsonReader.ReadObjectId(out timestamp, out machine, out pid, out increment);
      return new ObjectId(timestamp, machine, pid, increment).ToString();
    }

    public override object Deserialize(BsonReader bsonReader, Type nominalType, IBsonSerializationOptions options)
    {
      // read an ObjectId from the database but convert it to a string before returning it
      int timestamp, machine, increment;
      short pid;
      bsonReader.ReadObjectId(out timestamp, out machine, out pid, out increment);
      return new ObjectId(timestamp, machine, pid, increment).ToString();
    }

    public override void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
    {
      var objectId = new ObjectId((string)value);
      bsonWriter.WriteObjectId(objectId.Timestamp, objectId.Machine, objectId.Pid, objectId.Increment);
    }
  }
}

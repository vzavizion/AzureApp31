using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace AzureApp
{
    public class Person
    {
        [BsonId]
        [JsonIgnore]
        public ObjectId _id { get; set; }
        [BsonElement("id")]
        [BsonRepresentation(BsonType.Int32)]
        public int id { get; set; }
        public string name { get; set; }
    }
}

using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Qweree.Mongo
{
    public class MongoContext
    {
        static MongoContext()
        {
#pragma warning disable 618
            BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
#pragma warning restore 618
            BsonSerializer.RegisterSerializer(typeof(Guid), new GuidSerializer(GuidRepresentation.Standard));
        }

        private readonly IMongoDatabase _database;

        public MongoContext(string connectionString, string databaseName)
        {
            var mongoClient = new MongoClient(connectionString);
            _database = mongoClient.GetDatabase(databaseName);
        }

        public IMongoCollection<TDocumentType> GetCollection<TDocumentType>(string collectionName)
        {
            return _database.GetCollection<TDocumentType>(collectionName);
        }
    }
}
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Script
{
    public static class MongoHelper
    {
        // public static string ToJson(object obj)
        // {
        //     return obj.ToJson();
        // }

        // public static string ToJson(object obj, JsonWriterSettings settings)
        // {
        //     return obj.ToJson(settings);
        // }

        // public static T FromJson<T>(string str)
        // {
        //     try
        //     {
        //         return BsonSerializer.Deserialize<T>(str);
        //     }
        //     catch (Exception e)
        //     {
        //         throw new Exception($"{str}\n{e}");
        //     }
        // }

        // public static object FromJson(Type type, string str)
        // {
        //     return BsonSerializer.Deserialize(str, type);
        // }

        // public static byte[] ToBson(object obj)
        // {
        //     return obj.ToBson();
        // }

        // public static void ToStream(object message, MemoryStream stream)
        // {
        //     using (BsonBinaryWriter bsonWriter = new BsonBinaryWriter(stream, BsonBinaryWriterSettings.Defaults))
        //     {
        //         BsonSerializationContext context = BsonSerializationContext.CreateRoot(bsonWriter);
        //         BsonSerializationArgs args = default;
        //         args.NominalType = typeof(object);
        //         IBsonSerializer serializer = BsonSerializer.LookupSerializer(args.NominalType);
        //         serializer.Serialize(context, args, message);
        //     }
        // }

        // public static object FromBson(Type type, byte[] bytes)
        // {
        //     try
        //     {
        //         return BsonSerializer.Deserialize(bytes, type);
        //     }
        //     catch (Exception e)
        //     {
        //         throw new Exception($"from bson error: {type.Name}", e);
        //     }
        // }

        // public static object FromBson(Type type, byte[] bytes, int index, int count)
        // {
        //     try
        //     {
        //         using (MemoryStream memoryStream = new MemoryStream(bytes, index, count))
        //         {
        //             return BsonSerializer.Deserialize(memoryStream, type);
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         throw new Exception($"from bson error: {type.Name}", e);
        //     }
        // }

        // public static object FromStream(Type type, Stream stream)
        // {
        //     try
        //     {
        //         return BsonSerializer.Deserialize(stream, type);
        //     }
        //     catch (Exception e)
        //     {
        //         throw new Exception($"from bson error: {type.Name}", e);
        //     }
        // }

        // public static T FromBson<T>(byte[] bytes)
        // {
        //     try
        //     {
        //         using (MemoryStream memoryStream = new MemoryStream(bytes))
        //         {
        //             return (T)BsonSerializer.Deserialize(memoryStream, typeof(T));
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         throw new Exception($"from bson error: {typeof(T).Name}", e);
        //     }
        // }

        // public static T FromBson<T>(byte[] bytes, int index, int count)
        // {
        //     return (T)FromBson(typeof(T), bytes, index, count);
        // }

        // public static T Clone<T>(T t)
        // {
        //     return FromBson<T>(ToBson(t));
        // }

        public static async void InsertOneAsync<T>(MongoClient mongoClient, string dbName, string collectionName, T insertData)
        {
            IMongoDatabase dataBase = mongoClient.GetDatabase(dbName);
            var collection = dataBase.GetCollection<T>(collectionName);
            await collection.InsertOneAsync(insertData);
        }

        // https://www.mongodb.com/docs/manual/indexes/#single-field
        // For a single-field index and sort operations, the sort order (i.e. ascending or descending) of the index key does not matter because MongoDB can traverse the index in either direction.
        // 
        // The MongoCollection.CreateIndex method only creates an index if an index of the same specification does not already exist.

        public static async Task CreateIndex(MongoClient mongoClient, string dbName, string collectionName,
            string key, bool ascending, bool isUnique, log4net.ILog logger)
        {
            await CreateIndex(mongoClient, dbName, collectionName, new List<string>{ key }, ascending, isUnique, logger);
        }
        public static async Task CreateIndex(MongoClient mongoClient, string dbName, string collectionName,
            List<string> keys, bool ascending, bool isUnique, log4net.ILog logger)
        {
            var list = new List<IndexKeysDefinition<BsonDocument>>();
            foreach (var key in keys)
            {
                if (ascending)
                    list.Add(Builders<BsonDocument>.IndexKeys.Ascending(key));
                else
                    list.Add(Builders<BsonDocument>.IndexKeys.Descending(key));
            }

            IMongoDatabase dataBase = mongoClient.GetDatabase(dbName);
            var collection = dataBase.GetCollection<BsonDocument>(collectionName);

            var indexKeysDefinition = Builders<BsonDocument>.IndexKeys.Combine(list);
            string indexName = await collection.Indexes.CreateOneAsync(
                    new CreateIndexModel<BsonDocument>(indexKeysDefinition, new CreateIndexOptions() { Unique = isUnique, Background = true }));

            string keysString = null;
            if (keys.Count == 1)
            {
                keysString = keys[0];
            }
            else
            {
                keysString = "(";
                for (int i = 0; i < keys.Count; i++)
                {
                    keysString += keys[i];
                    if (i < keys.Count - 1)
                    {
                        keysString += ",";
                    }
                }
                keysString += ")";
            }

            // logger.InfoFormat("CreateIndex {0}.{1} => {2}", collectionName, keysString, indexName);
        }
    }
}
//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_arena_info : ServiceScript<NormalServer, DBPlayerService>
{
    public const string COLLECTION = "arena_info";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<ArenaInfo> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<ArenaInfo> collection = database.GetCollection<ArenaInfo>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task<ArenaInfo> Query_ArenaInfo_all()
    {
        var collection = this.GetCollection();
        var filter = Builders<ArenaInfo>.Filter.Empty;
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(ArenaInfo info)
    {
        var collection = this.GetCollection();
        var filter = Builders<ArenaInfo>.Filter.Empty;
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////

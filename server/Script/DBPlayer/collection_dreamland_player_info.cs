//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_dreamland_player_info : ServiceScript<NormalServer, DBPlayerService>
{
    public const string COLLECTION = "dreamland_player_info";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<DreamlandPlayerInfo> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<DreamlandPlayerInfo> collection = database.GetCollection<DreamlandPlayerInfo>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(DreamlandPlayerInfo.playerId), true, true, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<DreamlandPlayerInfo> Query_DreamlandPlayerInfo_by_playerId(longid playerId)
    {
        var collection = this.GetCollection();
        var filter = Builders<DreamlandPlayerInfo>.Filter.Eq(nameof(DreamlandPlayerInfo.playerId), playerId);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<List<DreamlandPlayerInfo>> Iterate_listOf_DreamlandPlayerInfo_by_playerId(longid start_playerId,longid end_playerId)
    {
        var collection = this.GetCollection();
        MyDebug.Assert(start_playerId < end_playerId);
        var gte = Builders<DreamlandPlayerInfo>.Filter.Gte(nameof(DreamlandPlayerInfo.playerId), start_playerId);
        var lt = Builders<DreamlandPlayerInfo>.Filter.Lt(nameof(DreamlandPlayerInfo.playerId), end_playerId);
        var filter = Builders<DreamlandPlayerInfo>.Filter.And(gte, lt);
        var find = collection.Find(filter)
            .Limit(1000);

        var result = await find.ToListAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(DreamlandPlayerInfo info)
    {
        var collection = this.GetCollection();
        var filter = Builders<DreamlandPlayerInfo>.Filter.Eq(nameof(DreamlandPlayerInfo.playerId), info.playerId);
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////

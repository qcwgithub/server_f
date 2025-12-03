//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_arena_player_info : ServiceScript<NormalServer, DBPlayerService>
{
    public const string COLLECTION = "arena_player_info";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<ArenaPlayerInfo> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<ArenaPlayerInfo> collection = database.GetCollection<ArenaPlayerInfo>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(ArenaPlayerInfo.playerId), true, true, this.service.logger);
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(ArenaPlayerInfo.matchRSeason), true, false, this.service.logger);
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(ArenaPlayerInfo.groupSeason), true, false, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<ArenaPlayerInfo> Query_ArenaPlayerInfo_by_playerId(longid playerId)
    {
        var collection = this.GetCollection();
        var filter = Builders<ArenaPlayerInfo>.Filter.Eq(nameof(ArenaPlayerInfo.playerId), playerId);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<List<ArenaPlayerInfo>> Iterate_listOf_ArenaPlayerInfo_by_playerId(longid start_playerId,longid end_playerId)
    {
        var collection = this.GetCollection();
        MyDebug.Assert(start_playerId < end_playerId);
        var gte = Builders<ArenaPlayerInfo>.Filter.Gte(nameof(ArenaPlayerInfo.playerId), start_playerId);
        var lt = Builders<ArenaPlayerInfo>.Filter.Lt(nameof(ArenaPlayerInfo.playerId), end_playerId);
        var filter = Builders<ArenaPlayerInfo>.Filter.And(gte, lt);
        var find = collection.Find(filter)
            .Limit(1000);

        var result = await find.ToListAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(ArenaPlayerInfo info)
    {
        var collection = this.GetCollection();
        var filter = Builders<ArenaPlayerInfo>.Filter.Eq(nameof(ArenaPlayerInfo.playerId), info.playerId);
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////

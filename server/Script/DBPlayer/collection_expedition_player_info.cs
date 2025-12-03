//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_expedition_player_info : ServiceScript<NormalServer, DBPlayerService>
{
    public const string COLLECTION = "expedition_player_info";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<ExpeditionPlayerInfo> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<ExpeditionPlayerInfo> collection = database.GetCollection<ExpeditionPlayerInfo>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(ExpeditionPlayerInfo.playerId), true, true, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<ExpeditionPlayerInfo> Query_ExpeditionPlayerInfo_by_playerId(longid playerId)
    {
        var collection = this.GetCollection();
        var filter = Builders<ExpeditionPlayerInfo>.Filter.Eq(nameof(ExpeditionPlayerInfo.playerId), playerId);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<List<ExpeditionPlayerInfo>> Iterate_listOf_ExpeditionPlayerInfo_by_playerId(longid start_playerId,longid end_playerId)
    {
        var collection = this.GetCollection();
        MyDebug.Assert(start_playerId < end_playerId);
        var gte = Builders<ExpeditionPlayerInfo>.Filter.Gte(nameof(ExpeditionPlayerInfo.playerId), start_playerId);
        var lt = Builders<ExpeditionPlayerInfo>.Filter.Lt(nameof(ExpeditionPlayerInfo.playerId), end_playerId);
        var filter = Builders<ExpeditionPlayerInfo>.Filter.And(gte, lt);
        var find = collection.Find(filter)
            .Limit(1000);

        var result = await find.ToListAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(ExpeditionPlayerInfo info)
    {
        var collection = this.GetCollection();
        var filter = Builders<ExpeditionPlayerInfo>.Filter.Eq(nameof(ExpeditionPlayerInfo.playerId), info.playerId);
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////

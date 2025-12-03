//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_expedition_party_player_info : ServiceScript<NormalServer, DBPlayerService>
{
    public const string COLLECTION = "expedition_party_player_info";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<ExpeditionPartyPlayerInfo> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<ExpeditionPartyPlayerInfo> collection = database.GetCollection<ExpeditionPartyPlayerInfo>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(ExpeditionPartyPlayerInfo.playerId), true, true, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<ExpeditionPartyPlayerInfo> Query_ExpeditionPartyPlayerInfo_by_playerId(longid playerId)
    {
        var collection = this.GetCollection();
        var filter = Builders<ExpeditionPartyPlayerInfo>.Filter.Eq(nameof(ExpeditionPartyPlayerInfo.playerId), playerId);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<List<ExpeditionPartyPlayerInfo>> Iterate_listOf_ExpeditionPartyPlayerInfo_by_playerId(longid start_playerId,longid end_playerId)
    {
        var collection = this.GetCollection();
        MyDebug.Assert(start_playerId < end_playerId);
        var gte = Builders<ExpeditionPartyPlayerInfo>.Filter.Gte(nameof(ExpeditionPartyPlayerInfo.playerId), start_playerId);
        var lt = Builders<ExpeditionPartyPlayerInfo>.Filter.Lt(nameof(ExpeditionPartyPlayerInfo.playerId), end_playerId);
        var filter = Builders<ExpeditionPartyPlayerInfo>.Filter.And(gte, lt);
        var find = collection.Find(filter)
            .Limit(1000);

        var result = await find.ToListAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(ExpeditionPartyPlayerInfo info)
    {
        var collection = this.GetCollection();
        var filter = Builders<ExpeditionPartyPlayerInfo>.Filter.Eq(nameof(ExpeditionPartyPlayerInfo.playerId), info.playerId);
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////

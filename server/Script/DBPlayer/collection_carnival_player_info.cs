//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_carnival_player_info : ServiceScript<NormalServer, DBPlayerService>
{
    public const string COLLECTION = "carnival_player_info";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<CarnivalPlayerInfo> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<CarnivalPlayerInfo> collection = database.GetCollection<CarnivalPlayerInfo>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(CarnivalPlayerInfo.playerId), true, true, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<CarnivalPlayerInfo> Query_CarnivalPlayerInfo_by_playerId(longid playerId)
    {
        var collection = this.GetCollection();
        var filter = Builders<CarnivalPlayerInfo>.Filter.Eq(nameof(CarnivalPlayerInfo.playerId), playerId);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<List<CarnivalPlayerInfo>> Iterate_listOf_CarnivalPlayerInfo_by_playerId(longid start_playerId,longid end_playerId)
    {
        var collection = this.GetCollection();
        MyDebug.Assert(start_playerId < end_playerId);
        var gte = Builders<CarnivalPlayerInfo>.Filter.Gte(nameof(CarnivalPlayerInfo.playerId), start_playerId);
        var lt = Builders<CarnivalPlayerInfo>.Filter.Lt(nameof(CarnivalPlayerInfo.playerId), end_playerId);
        var filter = Builders<CarnivalPlayerInfo>.Filter.And(gte, lt);
        var find = collection.Find(filter)
            .Limit(1000);

        var result = await find.ToListAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(CarnivalPlayerInfo info)
    {
        var collection = this.GetCollection();
        var filter = Builders<CarnivalPlayerInfo>.Filter.Eq(nameof(CarnivalPlayerInfo.playerId), info.playerId);
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////

//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_apex_player_info : ServiceScript<GroupServer, DBGroupService>
{
    public const string COLLECTION = "apex_player_info";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<ApexPlayerInfo> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<ApexPlayerInfo> collection = database.GetCollection<ApexPlayerInfo>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(ApexPlayerInfo.playerId), true, true, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<ApexPlayerInfo> Query_ApexPlayerInfo_by_playerId(longid playerId)
    {
        var collection = this.GetCollection();
        var filter = Builders<ApexPlayerInfo>.Filter.Eq(nameof(ApexPlayerInfo.playerId), playerId);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(ApexPlayerInfo info)
    {
        var collection = this.GetCollection();
        var filter = Builders<ApexPlayerInfo>.Filter.Eq(nameof(ApexPlayerInfo.playerId), info.playerId);
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////

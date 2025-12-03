//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;
using ApexPlayerBattleSide = Data.PlayerBattleSide;


//// AUTO CREATED ////
public  class collection_apex_player_battle_side : ServiceScript<GroupServer, DBGroupService>
{
    public const string COLLECTION = "apex_player_battle_side";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<ApexPlayerBattleSide> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<ApexPlayerBattleSide> collection = database.GetCollection<ApexPlayerBattleSide>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(ApexPlayerBattleSide.playerId), true, true, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<ApexPlayerBattleSide> Query_ApexPlayerBattleSide_by_playerId(longid playerId)
    {
        var collection = this.GetCollection();
        var filter = Builders<ApexPlayerBattleSide>.Filter.Eq(nameof(ApexPlayerBattleSide.playerId), playerId);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(ApexPlayerBattleSide info)
    {
        var collection = this.GetCollection();
        var filter = Builders<ApexPlayerBattleSide>.Filter.Eq(nameof(ApexPlayerBattleSide.playerId), info.playerId);
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////

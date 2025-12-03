//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_player_battle_side : ServiceScript<NormalServer, DBPlayerService>
{
    public const string COLLECTION = "player_battle_side";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<PlayerBattleSide> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<PlayerBattleSide> collection = database.GetCollection<PlayerBattleSide>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public IMongoCollection<PlayerBattleSide_Db> GetCollection_Db()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<PlayerBattleSide_Db> collection = database.GetCollection<PlayerBattleSide_Db>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(PlayerBattleSide.playerId), true, true, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<PlayerBattleSide> Query_PlayerBattleSide_by_playerId(longid playerId)
    {
        var collection = this.GetCollection();
        var filter = Builders<PlayerBattleSide>.Filter.Eq(nameof(PlayerBattleSide.playerId), playerId);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(PlayerBattleSide info)
    {
        var collection = this.GetCollection_Db();
        var filter = Builders<PlayerBattleSide_Db>.Filter.Eq(nameof(PlayerBattleSide_Db.playerId), info.playerId);
        var info_Db = ProfileHelper_Db.Copy_Class<PlayerBattleSide_Db, PlayerBattleSide>(info);
        await collection.ReplaceOneAsync(filter, info_Db, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////

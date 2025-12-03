//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;
using ChampionPlayerBattleSide = Data.PlayerBattleSide;
using ChampionPlayerBattleSide_Db = Data.PlayerBattleSide_Db;


//// AUTO CREATED ////
public  class collection_champion_player_battle_side : ServiceScript<NormalServer, DBPlayerService>
{
    public const string COLLECTION = "champion_player_battle_side";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<ChampionPlayerBattleSide> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<ChampionPlayerBattleSide> collection = database.GetCollection<ChampionPlayerBattleSide>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public IMongoCollection<ChampionPlayerBattleSide_Db> GetCollection_Db()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<ChampionPlayerBattleSide_Db> collection = database.GetCollection<ChampionPlayerBattleSide_Db>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(ChampionPlayerBattleSide.playerId), true, true, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<ChampionPlayerBattleSide> Query_ChampionPlayerBattleSide_by_playerId(longid playerId)
    {
        var collection = this.GetCollection();
        var filter = Builders<ChampionPlayerBattleSide>.Filter.Eq(nameof(ChampionPlayerBattleSide.playerId), playerId);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(ChampionPlayerBattleSide info)
    {
        var collection = this.GetCollection_Db();
        var filter = Builders<ChampionPlayerBattleSide_Db>.Filter.Eq(nameof(ChampionPlayerBattleSide_Db.playerId), info.playerId);
        var info_Db = ProfileHelper_Db.Copy_Class<ChampionPlayerBattleSide_Db, ChampionPlayerBattleSide>(info);
        await collection.ReplaceOneAsync(filter, info_Db, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////

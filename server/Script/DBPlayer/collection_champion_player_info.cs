//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_champion_player_info : ServiceScript<NormalServer, DBPlayerService>
{
    public const string COLLECTION = "champion_player_info";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<ChampionPlayerInfo> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<ChampionPlayerInfo> collection = database.GetCollection<ChampionPlayerInfo>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(ChampionPlayerInfo.playerId), true, true, this.service.logger);
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(ChampionPlayerInfo.groupSeason), true, false, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<ChampionPlayerInfo> Query_ChampionPlayerInfo_by_playerId(longid playerId)
    {
        var collection = this.GetCollection();
        var filter = Builders<ChampionPlayerInfo>.Filter.Eq(nameof(ChampionPlayerInfo.playerId), playerId);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<List<ChampionPlayerInfo>> Query_listOf_ChampionPlayerInfo_byListOf_playerId(List<longid> playerIdList)
    {
        var collection = this.GetCollection();
        var filter = Builders<ChampionPlayerInfo>.Filter.In(nameof(ChampionPlayerInfo.playerId), playerIdList);
        var find = collection.Find(filter);

        var result = await find.ToListAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<List<longid>> Query_listOf_ChampionPlayerInfo_playerId_by_groupSeason(int groupSeason)
    {
        var collection = this.GetCollection();
        var filter = Builders<ChampionPlayerInfo>.Filter.Eq(nameof(ChampionPlayerInfo.groupSeason), groupSeason);
        var projection = Builders<ChampionPlayerInfo>.Projection.Include(nameof(ChampionPlayerInfo.playerId));
        var find = collection.Find(filter)
            .Project(projection);

        var result = await find.ToListAsync();
        return result.Select(_ => (longid)_[nameof(ChampionPlayerInfo.playerId)]).ToList();
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(ChampionPlayerInfo info)
    {
        var collection = this.GetCollection();
        var filter = Builders<ChampionPlayerInfo>.Filter.Eq(nameof(ChampionPlayerInfo.playerId), info.playerId);
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////

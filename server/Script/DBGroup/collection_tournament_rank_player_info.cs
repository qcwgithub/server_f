//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_tournament_rank_player_info : ServiceScript<GroupServer, DBGroupService>
{
    public const string COLLECTION = "tournament_rank_player_info";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<TournamentRankPlayerInfo> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<TournamentRankPlayerInfo> collection = database.GetCollection<TournamentRankPlayerInfo>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(TournamentRankPlayerInfo.playerId), true, true, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<TournamentRankPlayerInfo> Query_TournamentRankPlayerInfo_by_playerId(longid playerId)
    {
        var collection = this.GetCollection();
        var filter = Builders<TournamentRankPlayerInfo>.Filter.Eq(nameof(TournamentRankPlayerInfo.playerId), playerId);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<List<TournamentRankPlayerInfo>> Iterate_listOf_TournamentRankPlayerInfo_by_playerId(longid start_playerId,longid end_playerId)
    {
        var collection = this.GetCollection();
        MyDebug.Assert(start_playerId < end_playerId);
        var gte = Builders<TournamentRankPlayerInfo>.Filter.Gte(nameof(TournamentRankPlayerInfo.playerId), start_playerId);
        var lt = Builders<TournamentRankPlayerInfo>.Filter.Lt(nameof(TournamentRankPlayerInfo.playerId), end_playerId);
        var filter = Builders<TournamentRankPlayerInfo>.Filter.And(gte, lt);
        var find = collection.Find(filter)
            .Limit(1000);

        var result = await find.ToListAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(TournamentRankPlayerInfo info)
    {
        var collection = this.GetCollection();
        var filter = Builders<TournamentRankPlayerInfo>.Filter.Eq(nameof(TournamentRankPlayerInfo.playerId), info.playerId);
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////

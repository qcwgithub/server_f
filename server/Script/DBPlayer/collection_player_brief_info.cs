//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_player_brief_info : ServiceScript<NormalServer, DBPlayerService>
{
    public const string COLLECTION = "player_brief";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<PlayerBriefInfo> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<PlayerBriefInfo> collection = database.GetCollection<PlayerBriefInfo>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(PlayerBriefInfo.playerId), true, true, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<PlayerBriefInfo> Query_PlayerBriefInfo_by_playerId(longid playerId)
    {
        var collection = this.GetCollection();
        var filter = Builders<PlayerBriefInfo>.Filter.Eq(nameof(PlayerBriefInfo.playerId), playerId);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<Dictionary<longid, PlayerBriefInfo>> Iterate_dictOf_PlayerBriefInfo_playerId_info_by_playerId(longid start_playerId,longid end_playerId)
    {
        var collection = this.GetCollection();
        MyDebug.Assert(start_playerId < end_playerId);
        var gte = Builders<PlayerBriefInfo>.Filter.Gte(nameof(PlayerBriefInfo.playerId), start_playerId);
        var lt = Builders<PlayerBriefInfo>.Filter.Lt(nameof(PlayerBriefInfo.playerId), end_playerId);
        var filter = Builders<PlayerBriefInfo>.Filter.And(gte, lt);
        var find = collection.Find(filter)
            .Limit(1000);

        var result = await find.ToListAsync();
        var dict = new Dictionary<longid, PlayerBriefInfo>();
        foreach (var item in result)
        {
            dict[item.playerId] = item;
        }
        return dict;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(PlayerBriefInfo info)
    {
        var collection = this.GetCollection();
        var filter = Builders<PlayerBriefInfo>.Filter.Eq(nameof(PlayerBriefInfo.playerId), info.playerId);
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////

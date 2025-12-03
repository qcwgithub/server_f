//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_player_union_info : ServiceScript<NormalServer, DBPlayerService>
{
    public const string COLLECTION = "player_union_info";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<PlayerUnionInfo> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<PlayerUnionInfo> collection = database.GetCollection<PlayerUnionInfo>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(PlayerUnionInfo.playerId), true, true, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<PlayerUnionInfo> Query_PlayerUnionInfo_by_playerId(longid playerId)
    {
        var collection = this.GetCollection();
        var filter = Builders<PlayerUnionInfo>.Filter.Eq(nameof(PlayerUnionInfo.playerId), playerId);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<List<longid>> Query_listOf_PlayerUnionInfo_playerId_by_unionId(longid unionId)
    {
        var collection = this.GetCollection();
        var filter = Builders<PlayerUnionInfo>.Filter.Eq(nameof(PlayerUnionInfo.unionId), unionId);
        var projection = Builders<PlayerUnionInfo>.Projection.Include(nameof(PlayerUnionInfo.playerId));
        var find = collection.Find(filter)
            .Project(projection);

        var result = await find.ToListAsync();
        return result.Select(_ => (longid)_[nameof(PlayerUnionInfo.playerId)]).ToList();
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(PlayerUnionInfo info)
    {
        var collection = this.GetCollection();
        var filter = Builders<PlayerUnionInfo>.Filter.Eq(nameof(PlayerUnionInfo.playerId), info.playerId);
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////

//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_worldmap_map_info : ServiceScript<NormalServer, DBPlayerService>
{
    public const string COLLECTION = "profile_worldmap_map_info";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<WorldMapMapInfo> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<WorldMapMapInfo> collection = database.GetCollection<WorldMapMapInfo>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(WorldMapMapInfo.mapId), true, true, this.service.logger);
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(WorldMapMapInfo.playerOrUnionId), true, false, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<WorldMapMapInfo> Query_WorldMapMapInfo_by_mapId(string mapId)
    {
        var collection = this.GetCollection();
        var filter = Builders<WorldMapMapInfo>.Filter.Eq(nameof(WorldMapMapInfo.mapId), mapId);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<List<WorldMapMapInfo>> Iterate_listOf_WorldMapMapInfo_by_playerOrUnionId(longid start_playerOrUnionId,longid end_playerOrUnionId)
    {
        var collection = this.GetCollection();
        MyDebug.Assert(start_playerOrUnionId < end_playerOrUnionId);
        var gte = Builders<WorldMapMapInfo>.Filter.Gte(nameof(WorldMapMapInfo.playerOrUnionId), start_playerOrUnionId);
        var lt = Builders<WorldMapMapInfo>.Filter.Lt(nameof(WorldMapMapInfo.playerOrUnionId), end_playerOrUnionId);
        var filter = Builders<WorldMapMapInfo>.Filter.And(gte, lt);
        var find = collection.Find(filter)
            .Limit(1000);

        var result = await find.ToListAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(WorldMapMapInfo info)
    {
        var collection = this.GetCollection();
        var filter = Builders<WorldMapMapInfo>.Filter.Eq(nameof(WorldMapMapInfo.mapId), info.mapId);
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////

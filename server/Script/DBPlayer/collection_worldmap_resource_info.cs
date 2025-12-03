//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_worldmap_resource_info : ServiceScript<NormalServer, DBPlayerService>
{
    public const string COLLECTION = "worldmap_resource_info";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<WorldMapResourceInfo> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<WorldMapResourceInfo> collection = database.GetCollection<WorldMapResourceInfo>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(WorldMapResourceInfo.mapId), true, false, this.service.logger);
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(WorldMapResourceInfo.resourceId), true, false, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<WorldMapResourceInfo> Query_WorldMapResourceInfo_by_mapId_resourceId(string mapId,string resourceId)
    {
        var collection = this.GetCollection();
        var eq1 = Builders<WorldMapResourceInfo>.Filter.Eq(nameof(WorldMapResourceInfo.mapId), mapId);
        var eq2 = Builders<WorldMapResourceInfo>.Filter.Eq(nameof(WorldMapResourceInfo.resourceId), resourceId);
        var filter = Builders<WorldMapResourceInfo>.Filter.And(eq1, eq2);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<List<WorldMapResourceInfo>> Query_listOf_WorldMapResourceInfo_by_mapId(string mapId)
    {
        var collection = this.GetCollection();
        var filter = Builders<WorldMapResourceInfo>.Filter.Eq(nameof(WorldMapResourceInfo.mapId), mapId);
        var find = collection.Find(filter);

        var result = await find.ToListAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(WorldMapResourceInfo info)
    {
        var collection = this.GetCollection();
        var eq1 = Builders<WorldMapResourceInfo>.Filter.Eq(nameof(WorldMapResourceInfo.mapId), info.mapId);
        var eq2 = Builders<WorldMapResourceInfo>.Filter.Eq(nameof(WorldMapResourceInfo.resourceId), info.resourceId);
        var filter = Builders<WorldMapResourceInfo>.Filter.And(eq1, eq2);
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////

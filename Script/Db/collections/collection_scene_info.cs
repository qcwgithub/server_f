//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;

//// AUTO CREATED ////
public partial  class collection_scene_info : ServiceScript<DbService>
{
    public const string COLLECTION = "scene_info";
    MongoClient mongoClient => this.server.data.mongoClient;
    string dbName => this.server.data.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public collection_scene_info(Server server, DbService service) : base(server, service)
    {
    }

    //// AUTO CREATED ////
    public IMongoCollection<SceneInfo> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<SceneInfo> collection = database.GetCollection<SceneInfo>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(SceneInfo.sceneId), true, true, this.service.logger);
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(SceneInfo.title), true, false, this.service.logger);
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(SceneInfo.desc), true, false, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<SceneInfo> Query_SceneInfo_by_sceneId(long sceneId)
    {
        var collection = this.GetCollection();
        var filter = Builders<SceneInfo>.Filter.Eq(nameof(SceneInfo.sceneId), sceneId);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<long> Query_SceneInfo_maxOf_sceneId()
    {
        var collection = this.GetCollection();
        var filter = Builders<SceneInfo>.Filter.Gt(nameof(SceneInfo.sceneId), 0);
        var projection = Builders<SceneInfo>.Projection.Include(nameof(SceneInfo.sceneId));
        var find = collection.Find(filter)
            .SortByDescending(x => x.sceneId)
            .Skip(0)
            .Limit(1)
            .Project(projection);

        var result = await find.ToListAsync();
        return result.Count > 0 ? (long)result[0][nameof(SceneInfo.sceneId)] : default(long);
    }
}

//// AUTO CREATED ////

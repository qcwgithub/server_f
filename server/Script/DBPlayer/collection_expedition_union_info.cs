//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_expedition_union_info : ServiceScript<NormalServer, DBPlayerService>
{
    public const string COLLECTION = "expedition_union_info";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<ExpeditionUnionInfo> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<ExpeditionUnionInfo> collection = database.GetCollection<ExpeditionUnionInfo>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(ExpeditionUnionInfo.unionId), true, true, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<ExpeditionUnionInfo> Query_ExpeditionUnionInfo_by_unionId(longid unionId)
    {
        var collection = this.GetCollection();
        var filter = Builders<ExpeditionUnionInfo>.Filter.Eq(nameof(ExpeditionUnionInfo.unionId), unionId);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<List<ExpeditionUnionInfo>> Iterate_listOf_ExpeditionUnionInfo_by_unionId(longid start_unionId,longid end_unionId)
    {
        var collection = this.GetCollection();
        MyDebug.Assert(start_unionId < end_unionId);
        var gte = Builders<ExpeditionUnionInfo>.Filter.Gte(nameof(ExpeditionUnionInfo.unionId), start_unionId);
        var lt = Builders<ExpeditionUnionInfo>.Filter.Lt(nameof(ExpeditionUnionInfo.unionId), end_unionId);
        var filter = Builders<ExpeditionUnionInfo>.Filter.And(gte, lt);
        var find = collection.Find(filter)
            .Limit(1000);

        var result = await find.ToListAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(ExpeditionUnionInfo info)
    {
        var collection = this.GetCollection();
        var filter = Builders<ExpeditionUnionInfo>.Filter.Eq(nameof(ExpeditionUnionInfo.unionId), info.unionId);
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////

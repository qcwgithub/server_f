//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_union_season_info : ServiceScript<GroupServer, DBGroupService>
{
    public const string COLLECTION = "union_season_info";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<UnionSeasonInfo> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<UnionSeasonInfo> collection = database.GetCollection<UnionSeasonInfo>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(UnionSeasonInfo.unionId), true, true, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<UnionSeasonInfo> Query_UnionSeasonInfo_by_unionId(longid unionId)
    {
        var collection = this.GetCollection();
        var filter = Builders<UnionSeasonInfo>.Filter.Eq(nameof(UnionSeasonInfo.unionId), unionId);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<List<longid>> Query_listOf_UnionSeasonInfo_unionId_all()
    {
        var collection = this.GetCollection();
        var filter = Builders<UnionSeasonInfo>.Filter.Empty;
        var projection = Builders<UnionSeasonInfo>.Projection.Include(nameof(UnionSeasonInfo.unionId));
        var find = collection.Find(filter)
            .Project(projection);

        var result = await find.ToListAsync();
        return result.Select(_ => (longid)_[nameof(UnionSeasonInfo.unionId)]).ToList();
    }

    //// AUTO CREATED ////
    public async Task<List<UnionSeasonInfo>> Iterate_listOf_UnionSeasonInfo_by_unionId(longid start_unionId,longid end_unionId)
    {
        var collection = this.GetCollection();
        MyDebug.Assert(start_unionId < end_unionId);
        var gte = Builders<UnionSeasonInfo>.Filter.Gte(nameof(UnionSeasonInfo.unionId), start_unionId);
        var lt = Builders<UnionSeasonInfo>.Filter.Lt(nameof(UnionSeasonInfo.unionId), end_unionId);
        var filter = Builders<UnionSeasonInfo>.Filter.And(gte, lt);
        var find = collection.Find(filter)
            .Limit(1000);

        var result = await find.ToListAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(UnionSeasonInfo info)
    {
        var collection = this.GetCollection();
        var filter = Builders<UnionSeasonInfo>.Filter.Eq(nameof(UnionSeasonInfo.unionId), info.unionId);
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////

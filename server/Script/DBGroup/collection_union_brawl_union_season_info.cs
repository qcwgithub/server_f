//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_union_brawl_union_season_info : ServiceScript<GroupServer, DBGroupService>
{
    public const string COLLECTION = "union_brawl_union_season_info";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<UnionBrawlUnionSeasonInfo> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<UnionBrawlUnionSeasonInfo> collection = database.GetCollection<UnionBrawlUnionSeasonInfo>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(UnionBrawlUnionSeasonInfo.unionId), true, true, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<UnionBrawlUnionSeasonInfo> Query_UnionBrawlUnionSeasonInfo_by_unionId(longid unionId)
    {
        var collection = this.GetCollection();
        var filter = Builders<UnionBrawlUnionSeasonInfo>.Filter.Eq(nameof(UnionBrawlUnionSeasonInfo.unionId), unionId);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<List<longid>> Query_listOf_UnionBrawlUnionSeasonInfo_unionId_all()
    {
        var collection = this.GetCollection();
        var filter = Builders<UnionBrawlUnionSeasonInfo>.Filter.Empty;
        var projection = Builders<UnionBrawlUnionSeasonInfo>.Projection.Include(nameof(UnionBrawlUnionSeasonInfo.unionId));
        var find = collection.Find(filter)
            .Project(projection);

        var result = await find.ToListAsync();
        return result.Select(_ => (longid)_[nameof(UnionBrawlUnionSeasonInfo.unionId)]).ToList();
    }

    //// AUTO CREATED ////
    public async Task<List<UnionBrawlUnionSeasonInfo>> Iterate_listOf_UnionBrawlUnionSeasonInfo_by_unionId(longid start_unionId,longid end_unionId)
    {
        var collection = this.GetCollection();
        MyDebug.Assert(start_unionId < end_unionId);
        var gte = Builders<UnionBrawlUnionSeasonInfo>.Filter.Gte(nameof(UnionBrawlUnionSeasonInfo.unionId), start_unionId);
        var lt = Builders<UnionBrawlUnionSeasonInfo>.Filter.Lt(nameof(UnionBrawlUnionSeasonInfo.unionId), end_unionId);
        var filter = Builders<UnionBrawlUnionSeasonInfo>.Filter.And(gte, lt);
        var find = collection.Find(filter)
            .Limit(1000);

        var result = await find.ToListAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(UnionBrawlUnionSeasonInfo info)
    {
        var collection = this.GetCollection();
        var filter = Builders<UnionBrawlUnionSeasonInfo>.Filter.Eq(nameof(UnionBrawlUnionSeasonInfo.unionId), info.unionId);
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////

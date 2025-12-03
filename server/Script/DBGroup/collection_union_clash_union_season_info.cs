//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_union_clash_union_season_info : ServiceScript<GroupServer, DBGroupService>
{
    public const string COLLECTION = "union_clash_union_season_info";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<UnionClashUnionSeasonInfo> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<UnionClashUnionSeasonInfo> collection = database.GetCollection<UnionClashUnionSeasonInfo>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(UnionClashUnionSeasonInfo.unionId), true, true, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<UnionClashUnionSeasonInfo> Query_UnionClashUnionSeasonInfo_by_unionId(longid unionId)
    {
        var collection = this.GetCollection();
        var filter = Builders<UnionClashUnionSeasonInfo>.Filter.Eq(nameof(UnionClashUnionSeasonInfo.unionId), unionId);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<List<UnionClashUnionSeasonInfo>> Iterate_listOf_UnionClashUnionSeasonInfo_by_unionId(longid start_unionId,longid end_unionId)
    {
        var collection = this.GetCollection();
        MyDebug.Assert(start_unionId < end_unionId);
        var gte = Builders<UnionClashUnionSeasonInfo>.Filter.Gte(nameof(UnionClashUnionSeasonInfo.unionId), start_unionId);
        var lt = Builders<UnionClashUnionSeasonInfo>.Filter.Lt(nameof(UnionClashUnionSeasonInfo.unionId), end_unionId);
        var filter = Builders<UnionClashUnionSeasonInfo>.Filter.And(gte, lt);
        var find = collection.Find(filter)
            .Limit(1000);

        var result = await find.ToListAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(UnionClashUnionSeasonInfo info)
    {
        var collection = this.GetCollection();
        var filter = Builders<UnionClashUnionSeasonInfo>.Filter.Eq(nameof(UnionClashUnionSeasonInfo.unionId), info.unionId);
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////

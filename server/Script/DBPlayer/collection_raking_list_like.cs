//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_raking_list_like : ServiceScript<NormalServer, DBPlayerService>
{
    public const string COLLECTION = "raking_list_like";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<RankingListLike> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<RankingListLike> collection = database.GetCollection<RankingListLike>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, new List<string> { nameof(RankingListLike.rankName), nameof(RankingListLike.memberId) }, true, true, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<List<RankingListLike>> Iterate_listOf_RankingListLike_by_memberId(longid start_memberId,longid end_memberId)
    {
        var collection = this.GetCollection();
        MyDebug.Assert(start_memberId < end_memberId);
        var gte = Builders<RankingListLike>.Filter.Gte(nameof(RankingListLike.memberId), start_memberId);
        var lt = Builders<RankingListLike>.Filter.Lt(nameof(RankingListLike.memberId), end_memberId);
        var filter = Builders<RankingListLike>.Filter.And(gte, lt);
        var find = collection.Find(filter)
            .Limit(1000);

        var result = await find.ToListAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(RankingListLike info)
    {
        var collection = this.GetCollection();
        var eq1 = Builders<RankingListLike>.Filter.Eq(nameof(RankingListLike.rankName), info.rankName);
        var eq2 = Builders<RankingListLike>.Filter.Eq(nameof(RankingListLike.memberId), info.memberId);
        var filter = Builders<RankingListLike>.Filter.And(eq1, eq2);
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////

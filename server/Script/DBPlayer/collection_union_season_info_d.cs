//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_union_season_info_d : ServiceScript<NormalServer, DBPlayerService>
{
    public const string COLLECTION = "union_season_info_d";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<UnionSeasonInfoD> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<UnionSeasonInfoD> collection = database.GetCollection<UnionSeasonInfoD>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(UnionSeasonInfoD.unionId), true, true, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<UnionSeasonInfoD> Query_UnionSeasonInfoD_by_unionId(longid unionId)
    {
        var collection = this.GetCollection();
        var filter = Builders<UnionSeasonInfoD>.Filter.Eq(nameof(UnionSeasonInfoD.unionId), unionId);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<List<UnionSeasonInfoD>> Iterate_listOf_UnionSeasonInfoD_by_unionId(longid start_unionId,longid end_unionId)
    {
        var collection = this.GetCollection();
        MyDebug.Assert(start_unionId < end_unionId);
        var gte = Builders<UnionSeasonInfoD>.Filter.Gte(nameof(UnionSeasonInfoD.unionId), start_unionId);
        var lt = Builders<UnionSeasonInfoD>.Filter.Lt(nameof(UnionSeasonInfoD.unionId), end_unionId);
        var filter = Builders<UnionSeasonInfoD>.Filter.And(gte, lt);
        var find = collection.Find(filter)
            .Limit(1000);

        var result = await find.ToListAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(UnionSeasonInfoD info)
    {
        var collection = this.GetCollection();
        var filter = Builders<UnionSeasonInfoD>.Filter.Eq(nameof(UnionSeasonInfoD.unionId), info.unionId);
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////

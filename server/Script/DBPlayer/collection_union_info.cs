//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_union_info : ServiceScript<NormalServer, DBPlayerService>
{
    public const string COLLECTION = "union_info";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<UnionInfo> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<UnionInfo> collection = database.GetCollection<UnionInfo>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(UnionInfo.unionId), true, true, this.service.logger);
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(UnionInfo.deleted), true, false, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<UnionInfo> Query_UnionInfo_by_unionId(longid unionId)
    {
        var collection = this.GetCollection();
        var filter = Builders<UnionInfo>.Filter.Eq(nameof(UnionInfo.unionId), unionId);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<longid> Query_UnionInfo_maxOf_unionId_by_serverId(int serverId)
    {
        var collection = this.GetCollection();
        var gte = Builders<UnionInfo>.Filter.Gte(nameof(UnionInfo.unionId), serverId * longidext.N);
        var lt = Builders<UnionInfo>.Filter.Lt(nameof(UnionInfo.unionId), (serverId + 1) * longidext.N);
        var filter = Builders<UnionInfo>.Filter.And(gte, lt);
        var projection = Builders<UnionInfo>.Projection.Include(nameof(UnionInfo.unionId));
        var find = collection.Find(filter)
            .SortByDescending(x => x.unionId)
            .Skip(0)
            .Limit(1)
            .Project(projection);

        var result = await find.ToListAsync();
        return result.Count > 0 ? (longid)result[0][nameof(UnionInfo.unionId)] : default(longid);
    }

    //// AUTO CREATED ////
    public async Task<List<UnionInfo>> Iterate_listOf_UnionInfo_by_unionId(longid start_unionId,longid end_unionId)
    {
        var collection = this.GetCollection();
        MyDebug.Assert(start_unionId < end_unionId);
        var gte = Builders<UnionInfo>.Filter.Gte(nameof(UnionInfo.unionId), start_unionId);
        var lt = Builders<UnionInfo>.Filter.Lt(nameof(UnionInfo.unionId), end_unionId);
        var filter = Builders<UnionInfo>.Filter.And(gte, lt);
        var find = collection.Find(filter)
            .Limit(1000);

        var result = await find.ToListAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<Dictionary<longid, UnionInfo_name_shortName>> Iterate_dictOf_UnionInfo_unionId_name_shortName_by_unionId(longid start_unionId,longid end_unionId)
    {
        var collection = this.GetCollection();
        MyDebug.Assert(start_unionId < end_unionId);
        var gte = Builders<UnionInfo>.Filter.Gte(nameof(UnionInfo.unionId), start_unionId);
        var lt = Builders<UnionInfo>.Filter.Lt(nameof(UnionInfo.unionId), end_unionId);
        var filter = Builders<UnionInfo>.Filter.And(gte, lt);
        var projection = Builders<UnionInfo>.Projection.Include(nameof(UnionInfo.unionId)).Include(nameof(UnionInfo.name)).Include(nameof(UnionInfo.shortName));
        var find = collection.Find(filter)
            .Limit(1000)
            .Project(projection);

        var result = await find.ToListAsync();
        var dict = new Dictionary<longid, UnionInfo_name_shortName>();
        foreach (var item in result)
        {
            longid unionId = (longid)item[nameof(UnionInfo.unionId)];
            string name = (string)item[nameof(UnionInfo.name)];
            string shortName = (string)item[nameof(UnionInfo.shortName)];
            dict[unionId] = new UnionInfo_name_shortName { name = name, shortName = shortName };
        }
        return dict;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(UnionInfo info)
    {
        var collection = this.GetCollection();
        var filter = Builders<UnionInfo>.Filter.Eq(nameof(UnionInfo.unionId), info.unionId);
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////

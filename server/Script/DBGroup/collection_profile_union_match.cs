//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_profile_union_match : ServiceScript<GroupServer, DBGroupService>
{
    public const string COLLECTION = "profile_union_match";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<ProfileUnionMatch> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<ProfileUnionMatch> collection = database.GetCollection<ProfileUnionMatch>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(ProfileUnionMatch.matchId), true, true, this.service.logger);
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(ProfileUnionMatch.season), true, false, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<ProfileUnionMatch> Query_ProfileUnionMatch_by_matchId(longid matchId)
    {
        var collection = this.GetCollection();
        var filter = Builders<ProfileUnionMatch>.Filter.Eq(nameof(ProfileUnionMatch.matchId), matchId);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<List<ProfileUnionMatch>> Query_listOf_ProfileUnionMatch_by_season(int season)
    {
        var collection = this.GetCollection();
        var filter = Builders<ProfileUnionMatch>.Filter.Eq(nameof(ProfileUnionMatch.season), season);
        var find = collection.Find(filter);

        var result = await find.ToListAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<longid> Query_ProfileUnionMatch_maxOf_matchId()
    {
        var collection = this.GetCollection();
        var filter = Builders<ProfileUnionMatch>.Filter.Gt(nameof(ProfileUnionMatch.matchId), 0);
        var projection = Builders<ProfileUnionMatch>.Projection.Include(nameof(ProfileUnionMatch.matchId));
        var find = collection.Find(filter)
            .SortByDescending(x => x.matchId)
            .Skip(0)
            .Limit(1)
            .Project(projection);

        var result = await find.ToListAsync();
        return result.Count > 0 ? (longid)result[0][nameof(ProfileUnionMatch.matchId)] : default(longid);
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(ProfileUnionMatch info)
    {
        var collection = this.GetCollection();
        var filter = Builders<ProfileUnionMatch>.Filter.Eq(nameof(ProfileUnionMatch.matchId), info.matchId);
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////

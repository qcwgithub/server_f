//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_profile_apex_group : ServiceScript<GroupServer, DBGroupService>
{
    public const string COLLECTION = "profile_apex_group";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<ProfileApexGroup> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<ProfileApexGroup> collection = database.GetCollection<ProfileApexGroup>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(ProfileApexGroup.groupId), true, true, this.service.logger);
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(ProfileApexGroup.season), true, false, this.service.logger);
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(ProfileApexGroup.grade), true, false, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<ProfileApexGroup> Query_ProfileApexGroup_by_groupId(longid groupId)
    {
        var collection = this.GetCollection();
        var filter = Builders<ProfileApexGroup>.Filter.Eq(nameof(ProfileApexGroup.groupId), groupId);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<longid> Query_ProfileApexGroup_maxOf_groupId()
    {
        var collection = this.GetCollection();
        var filter = Builders<ProfileApexGroup>.Filter.Gt(nameof(ProfileApexGroup.groupId), 0);
        var projection = Builders<ProfileApexGroup>.Projection.Include(nameof(ProfileApexGroup.groupId));
        var find = collection.Find(filter)
            .SortByDescending(x => x.groupId)
            .Skip(0)
            .Limit(1)
            .Project(projection);

        var result = await find.ToListAsync();
        return result.Count > 0 ? (longid)result[0][nameof(ProfileApexGroup.groupId)] : default(longid);
    }

    //// AUTO CREATED ////
    public async Task<List<ProfileApexGroup>> Query_listOf_ProfileApexGroup_by_season(int season)
    {
        var collection = this.GetCollection();
        var filter = Builders<ProfileApexGroup>.Filter.Eq(nameof(ProfileApexGroup.season), season);
        var find = collection.Find(filter);

        var result = await find.ToListAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(ProfileApexGroup info)
    {
        var collection = this.GetCollection();
        var filter = Builders<ProfileApexGroup>.Filter.Eq(nameof(ProfileApexGroup.groupId), info.groupId);
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////

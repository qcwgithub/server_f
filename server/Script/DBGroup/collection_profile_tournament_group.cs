//// AUTO CREATED ////
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using MongoDB.Driver;
using Script;
using System.Linq;
using longid = System.Int64;

//// AUTO CREATED ////
public  class collection_profile_tournament_group : ServiceScript<GroupServer, DBGroupService>
{
    public const string COLLECTION = "profile_tournament_group";
    MongoClient mongoClient => this.server.serverData.mongoClient;
    string dbName => this.server.serverData.mongoDBConfig.dbData;

    //// AUTO CREATED ////
    public IMongoCollection<ProfileTournamentGroup> GetCollection()
    {
        // It’s ok if the database doesn’t yet exist. It will be created upon first use.
        IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);
        IMongoCollection<ProfileTournamentGroup> collection = database.GetCollection<ProfileTournamentGroup>(COLLECTION);
        return collection;
    }

    //// AUTO CREATED ////
    public async Task CreateIndex()
    {
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(ProfileTournamentGroup.groupId), true, true, this.service.logger);
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(ProfileTournamentGroup.season), true, false, this.service.logger);
        await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(ProfileTournamentGroup.grade), true, false, this.service.logger);
    }

    //// AUTO CREATED ////
    public async Task<ProfileTournamentGroup> Query_ProfileTournamentGroup_by_groupId(longid groupId)
    {
        var collection = this.GetCollection();
        var filter = Builders<ProfileTournamentGroup>.Filter.Eq(nameof(ProfileTournamentGroup.groupId), groupId);
        var find = collection.Find(filter);

        var result = await find.FirstOrDefaultAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<longid> Query_ProfileTournamentGroup_maxOf_groupId()
    {
        var collection = this.GetCollection();
        var filter = Builders<ProfileTournamentGroup>.Filter.Gt(nameof(ProfileTournamentGroup.groupId), 0);
        var projection = Builders<ProfileTournamentGroup>.Projection.Include(nameof(ProfileTournamentGroup.groupId));
        var find = collection.Find(filter)
            .SortByDescending(x => x.groupId)
            .Skip(0)
            .Limit(1)
            .Project(projection);

        var result = await find.ToListAsync();
        return result.Count > 0 ? (longid)result[0][nameof(ProfileTournamentGroup.groupId)] : default(longid);
    }

    //// AUTO CREATED ////
    public async Task<List<ProfileTournamentGroup>> Query_listOf_ProfileTournamentGroup_by_season(int season)
    {
        var collection = this.GetCollection();
        var filter = Builders<ProfileTournamentGroup>.Filter.Eq(nameof(ProfileTournamentGroup.season), season);
        var find = collection.Find(filter);

        var result = await find.ToListAsync();
        return result;
    }

    //// AUTO CREATED ////
    public async Task<ECode> Save(ProfileTournamentGroup info)
    {
        var collection = this.GetCollection();
        var filter = Builders<ProfileTournamentGroup>.Filter.Eq(nameof(ProfileTournamentGroup.groupId), info.groupId);
        await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });
        return ECode.Success;
    }
}

//// AUTO CREATED ////
